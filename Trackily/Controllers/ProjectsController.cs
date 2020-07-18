using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Binding;
using Trackily.Models.Binding.Project;
using Trackily.Models.Views.Project;
using Trackily.Services.Business;

namespace Trackily.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ProjectService _projectService;
        private readonly IAuthorizationService _authService;

        public ProjectsController(ProjectService projectService, IAuthorizationService authService)
        {
            _projectService = projectService;
            _authService = authService;
        }

        // GET: Projects
        public ActionResult Index()
        {
            List<IndexProjectViewModel> projects = _projectService.CreateIndexProjectViewModels();
            return View(projects);
        }

        // GET: Projects/Details/5
        public async Task<ActionResult> Details(Guid projectId)
        {
            var authResult = await _authService.AuthorizeAsync(HttpContext.User, projectId, "ProjectEditPrivileges");
            if (!authResult.Succeeded)
            {
                return new ForbidResult();
            }

            var viewModel = _projectService.CreateDetailsProjectViewModel(projectId);
            return View(viewModel);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            var viewModel = _projectService.CreateProjectViewModel();
            return View(viewModel);
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BaseProjectBinding form)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> errors = ModelState.Values.SelectMany(v => v.Errors);
                var viewModel = _projectService.CreateProjectViewModel(form, errors);
                return View(viewModel);
            }

            await _projectService.CreateProject(form, HttpContext);
            return RedirectToAction(nameof(Index));
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(Guid projectId)
        {
            var viewModel = _projectService.CreateEditProjectViewModel(projectId);
            return View(viewModel);
        }

        // POST: Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditProjectBinding form)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> errors = ModelState.Values.SelectMany(v => v.Errors);
                var viewModel = _projectService.EditProjectViewModel(form, errors);
                return View(viewModel);
            }

            _projectService.EditProject(form);
            return RedirectToAction(nameof(Edit), new {projectId = form.ProjectId});
        }

        // Projects/Delete/5
        
        public async Task<ActionResult> Delete(Guid projectId)
        {
            var authResult = await _authService.AuthorizeAsync(HttpContext.User, projectId, "ProjectDeletePrivileges");
            if (!authResult.Succeeded)
            {
                return new ForbidResult();
            }

            _projectService.DeleteProject(projectId);
            return RedirectToAction(nameof(Index));
        }
    }
}
