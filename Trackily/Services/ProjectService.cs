using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Binding.Project;
using Trackily.Models.Domain;
using Trackily.Models.View.Project;

namespace Trackily.Services
{
    public class ProjectService
    {
        private readonly TrackilyContext _context;
        private readonly UserProjectService _userProjectService;
        private readonly UserManager<TrackilyUser> _userManager;
        private readonly UserTicketService _userTicketService;

        public ProjectService(
            TrackilyContext context,
            UserProjectService userProjectService,
            UserManager<TrackilyUser> userManager,
            UserTicketService userTicketService)
        {
            _context = context;
            _userProjectService = userProjectService;
            _userManager = userManager;
            _userTicketService = userTicketService;
        }

        public ProjectCreateViewModel CreateProjectViewModel(
            bool redirected,
            ProjectCreateBindingModel binding = null,
            IEnumerable<ModelError> errors = null)
        {
            var viewModel = new ProjectCreateViewModel();
            viewModel.Redirected = redirected;

            if (binding != null)
            {
                viewModel.ProjectId = binding.ProjectId;
                viewModel.Title = binding.Title;
                viewModel.Description = binding.Description;
                viewModel.AddMembers = binding.AddMembers;
            }

            if (errors != null)
            {
                viewModel.Errors = new List<string>();
                foreach (var error in errors)
                {
                    viewModel.Errors.Add(error.ErrorMessage);
                }
            }

            return viewModel;
        }

        public async Task CreateProject(ProjectCreateBindingModel form, HttpContext request)
        {
            var project = new Project
            {
                ProjectId = form.ProjectId,
                CreatedDate = DateTime.Now,
                Title = form.Title,
                Description = form.Description,
                Creator = await _userManager.GetUserAsync(request.User),
                Tickets = new List<Ticket>(),
                Members = new List<UserProject>()
            };

            if (!form.AddMembers.Contains(project.Creator.UserName)) // In case user still adds themselves as a member.
            {
                form.AddMembers.Add(project.Creator.UserName);
            }

            _userProjectService.AddMembersToProject(form.AddMembers, project);

            _context.Projects.Add(project);
            _context.SaveChanges(true);
        }

        public List<ProjectIndexViewModel> CreateIndexProjectViewModels()
        {
            List<Project> projects = _context.Projects
                                        .Include(p => p.Creator)
                                        .Include(p => p.Members)
                                        .Include(p => p.Tickets)
                                        .ToList();

            var viewModels = new List<ProjectIndexViewModel>();

            foreach (var project in projects)
            {
                var reloadProject = _context.Projects.Include(p => p.Creator)
                    .Single(p => p.ProjectId == project.ProjectId);

                var viewModel = new ProjectIndexViewModel()
                {
                    CreatedDate = project.CreatedDate,
                    CreatorName = $"{reloadProject.Creator.FirstName} {reloadProject.Creator.LastName}",
                    NumMembers = project.Members.Count,
                    NumTickets = project.Tickets.Count,
                    ProjectId = project.ProjectId,
                    Title = project.Title,
                    Description = project.Description
                };

                viewModels.Add(viewModel);
            }

            return viewModels;
        }

        public ProjectEditViewModel CreateEditProjectViewModel(
            Guid projectId,
            ProjectEditBindingModel binding = null,
            IEnumerable<ModelError> errors = null)
        {
            var project = _context.Projects
                .Include(p => p.Members)
                    .ThenInclude(m => m.User)
                .Single(p => p.ProjectId == projectId);

            if (binding == null && errors == null)
            {
                return EditProjectViewModel(project);
            }

            // Otherwise return a view model populated with the form data and errors.
            var viewModel = EditProjectViewModel(project);

            if (binding != null)
            {
                viewModel.Title = binding.Title;
                viewModel.Description = binding.Description;
                viewModel.AddMembers = binding.AddMembers;
            }

            if (errors != null)
            {
                viewModel.Errors = new List<string>();
                foreach (var error in errors)
                {
                    viewModel.Errors.Add(error.ErrorMessage);
                }
            }

            return viewModel;
        }

        private ProjectEditViewModel EditProjectViewModel(Project project)
        {
            var viewModel = new ProjectEditViewModel()
            {
                CreatedDate = project.CreatedDate,
                Description = project.Description,
                ProjectId = project.ProjectId,
                Title = project.Title,
                ExistingMembers = new List<Tuple<string, string>>(),
                RemoveMembers = new Dictionary<string, bool>()
            };

            viewModel.ExistingMembers = GetNamesForMembers(project);

            foreach (var username in project.Members.Select(up => up.User.UserName))
            {
                if (username == project.Creator.UserName)
                {
                    continue; // Do not show the Creator so that they cannot be removed from the Project.
                }
                viewModel.RemoveMembers.Add(username, false);
            }

            return viewModel;
        }

        public void EditProject(ProjectEditBindingModel form)
        {
            var project = _context.Projects
                                .Include(p => p.Members)
                                .Single(p => p.ProjectId == form.ProjectId);

            project.Title = form.Title;
            project.Description = form.Description;

            _userProjectService.AddMembersToProject(form.AddMembers, project);

            if (form.RemoveMembers != null)
            {
                // Remove UserProjects.
                var usernamesToRemove = form.RemoveMembers.Where(m => m.Value == true)
                                                                            .Select(m => m.Key) // Username.
                                                                            .ToList();
                _userProjectService.RemoveMembersFromProject(usernamesToRemove, project);

                // Remove UserTickets.
                foreach (var username in usernamesToRemove)
                {
                    var user = _context.Users.Single(u => u.UserName == username);
                    Debug.Assert(user != null);
                    _userTicketService.RemoveTicketsFromProject(user, project);
                }
            }

            _context.SaveChanges(true);
        }

        public ProjectDetailsViewModel CreateDetailsProjectViewModel(Guid projectId)
        {
            var project = _context.Projects
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.Assigned)
                .Include(p => p.Members)
                    .ThenInclude(ut => ut.User)
                .Single(p => p.ProjectId == projectId);

            var viewModel = new ProjectDetailsViewModel()
            {
                ProjectId = projectId,
                Title = project.Title,
                Description = project.Description,
                CreatedDate = project.CreatedDate,
                Tickets = new List<Ticket>(),
                Managers = new List<Tuple<string, string>>(),
                Developers = new List<Tuple<string, string>>()
            };

            viewModel.Tickets.AddRange(project.Tickets);

            var managers = _context.UserProjects
                .Include(up => up.User)
                .Where(up => up.ProjectId == projectId && up.User.Role == TrackilyUser.UserRole.Manager)
                .Select(up => up.User)
                .ToList();

            var developers = _context.UserProjects
                .Include(up => up.User)
                .Where(up => up.ProjectId == projectId && up.User.Role == TrackilyUser.UserRole.Developer)
                .Select(up => up.User)
                .ToList();

            viewModel.Managers = GetNamesOfUsers(managers);
            viewModel.Developers = GetNamesOfUsers(developers);

            return viewModel;
        }

        public void DeleteProject(Guid projectId)
        {
            var project = _context.Projects.Find(projectId);
            _context.Remove(project);
            _context.SaveChanges(true);
        }

        public List<Project> GetProjectsForUserId(Guid userId)
        {
            return _context.UserProjects.Include(up => up.Project)
                                        .ThenInclude(p => p.Tickets)
                                        .Where(up => up.Id == userId)
                                        .Select(up => up.Project)
                                        .ToList();
        }

        private List<Tuple<string, string>> GetNamesForMembers(Project project)
        {
            var members = new List<Tuple<string, string>>();

            foreach (var member in project.Members)
            {
                string name = $"{member.User.FirstName} {member.User.LastName}";
                var memberTuple = Tuple.Create(name, member.User.UserName);
                members.Add(memberTuple);
            }

            return members;
        }

        private List<Tuple<string, string>> GetNamesOfUsers(ICollection<TrackilyUser> users)
        {
            var names = new List<Tuple<string, string>>();

            foreach (var user in users)
            {
                string name = $"{user.FirstName} {user.LastName}";
                var memberTuple = Tuple.Create(name, user.UserName);
                names.Add(memberTuple);
            }

            return names;
        }
    }
}
