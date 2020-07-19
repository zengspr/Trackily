using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Binding.Project;
using Trackily.Models.Domain;
using Trackily.Models.Views.Project;
using Trackily.Services.DataAccess;

namespace Trackily.Services.Business
{
    public class ProjectService
    {
        private readonly TrackilyContext _context;
        private readonly UserProjectService _userProjectService;
        private readonly DbService _dbService;
        private readonly UserManager<TrackilyUser> _userManager;

        public ProjectService(TrackilyContext context,
            UserProjectService userProjectService, DbService dbService, UserManager<TrackilyUser> userManager)
        {
            _context = context;
            _userProjectService = userProjectService;
            _dbService = dbService;
            _userManager = userManager;
        }

        public CreateProjectViewModel CreateProjectViewModel(
            BaseProjectBinding binding = null,
            IEnumerable<ModelError> errors = null)
        {
            var viewModel = new CreateProjectViewModel();

            if (binding != null)
            {
                viewModel.ProjectId = binding.ProjectId;
                viewModel.Title = binding.Title;
                viewModel.Description = binding.Description;
                viewModel.AddMembers = binding.AddMembers;
            }

            if (errors != null)
            {
                foreach (var error in errors)
                {
                    viewModel.Errors.Add(error.ErrorMessage);
                }
            }

            return viewModel;
        }

        // TODO: Refactor methods for generating view models when there are model errors.
        public EditProjectViewModel EditProjectViewModel(
            EditProjectBinding binding = null,
            IEnumerable<ModelError> errors = null)
        {
            var viewModel = new EditProjectViewModel();
            var project = _context.Projects
                                .Include(p => p.Members)
                                    .ThenInclude(m => m.User)
                                .Single(p => p.ProjectId == binding.ProjectId);

            if (binding != null)
            {
                viewModel.ProjectId = binding.ProjectId;
                viewModel.Title = binding.Title;
                viewModel.Description = binding.Description;
                viewModel.AddMembers = binding.AddMembers;
                viewModel.Members = GetNamesForMembers(project);
            }

            if (errors != null)
            {
                foreach (var error in errors)
                {
                    viewModel.Errors.Add(error.ErrorMessage);
                }
            }

            return viewModel;
        }

        public List<IndexProjectViewModel> CreateIndexProjectViewModels()
        {
            List<Project> projects = _context.Projects
                                        .Include(p => p.Creator)
                                        .Include(p => p.Members)
                                        .Include(p => p.Tickets)
                                        .ToList();

            var viewModels = new List<IndexProjectViewModel>();

            foreach (var project in projects)
            {
                var viewModel = new IndexProjectViewModel()
                {
                    CreatedDate = project.CreatedDate,
                    CreatorName = _dbService.GetCreatorName(project),
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

        public async Task CreateProject(BaseProjectBinding form, HttpContext request)
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

            _userProjectService.AddMembersToProject(form.AddMembers, project);

            _context.Projects.Add(project);
            _context.SaveChanges(true);
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

        public DetailsProjectViewModel CreateDetailsProjectViewModel(Guid projectId)
        {
            var project = _context.Projects
                                .Include(p => p.Tickets)
                                    .ThenInclude(t => t.Assigned)
                                .Include(p => p.Members)
                                    .ThenInclude(ut => ut.User)
                                .Single(p => p.ProjectId == projectId);

            var viewModel = new DetailsProjectViewModel()
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

        public EditProjectViewModel CreateEditProjectViewModel(Guid projectId)
        {
            var project = _context.Projects
                                .Include(p => p.Members)
                                    .ThenInclude(ut => ut.User)
                                .Single(p => p.ProjectId == projectId);

            var viewModel = new EditProjectViewModel()
            {
                CreatedDate = project.CreatedDate,
                Description = project.Description,
                ProjectId = project.ProjectId,
                Title = project.Title,
                Members = new List<Tuple<string, string>>()
            };

            viewModel.Members = GetNamesForMembers(project);

            return viewModel;
        }

        public void EditProject(EditProjectBinding form)
        {
            var project = _context.Projects
                                .Include(p => p.Members)
                                .Single(p => p.ProjectId == form.ProjectId);

            project.Title = form.Title;
            project.Description = form.Description;

            _userProjectService.AddMembersToProject(form.AddMembers, project);

            _context.SaveChanges(true);
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
    }
}
