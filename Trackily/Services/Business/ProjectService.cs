using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Binding;
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
                Tickets = new List<Ticket>()
            };
            
            project.Members = _userProjectService.CreateUserProjectsForNames(form.AddMembers, project);

            _context.Projects.Add(project);
            _context.SaveChanges();
        }
    }
}
