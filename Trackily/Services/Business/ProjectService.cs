using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Binding;
using Trackily.Models.Domain;
using Trackily.Models.View;

namespace Trackily.Services.Business
{
    public class ProjectService
    {
        private readonly TrackilyContext _context;
        private readonly UserProjectService _userProjectService;

        public ProjectService(TrackilyContext context, UserProjectService userProjectService)
        {
            _context = context;
            _userProjectService = userProjectService;
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

        public void CreateProject(BaseProjectBinding form)
        {
            var project = new Project
            {
                ProjectId = form.ProjectId,
                CreatedDate = DateTime.Now,
                Title = form.Title,
                Description = form.Description,
                Tickets = new List<Ticket>()
            };
            
            project.Users = _userProjectService.CreateUserProjectsForNames(form.AddMembers, project);

            _context.Projects.Add(project);
            _context.SaveChanges();
        }
    }
}
