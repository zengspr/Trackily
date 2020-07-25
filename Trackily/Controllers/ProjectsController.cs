using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Models.Binding.Project;
using Trackily.Models.View.Project;
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
            List<ProjectIndexViewModel> projects = _projectService.CreateIndexProjectViewModels();
            return View(projects);
        }

        // GET: Projects/Create
        // Redirected indicates whether or not the user attempted to create a new ticket without an existing project. 
        public ActionResult Create(bool redirected)
        {
            var viewModel = _projectService.CreateProjectViewModel(redirected);
            return View(viewModel);
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProjectCreateBindingModel form)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> errors = ModelState.Values.SelectMany(v => v.Errors);
                var viewModel = _projectService.CreateProjectViewModel(false, form, errors);
                return View(viewModel);
            }

            await _projectService.CreateProject(form, HttpContext);
            return RedirectToAction(nameof(Index));
        }

        // GET: Projects/Details/5
        public async Task<ActionResult> Details(Guid projectId)
        {
            var authResult = await _authService.AuthorizeAsync(HttpContext.User, projectId, "ProjectDetailsPrivileges");
            if (!authResult.Succeeded)
            {
                return new ForbidResult();
            }

            var viewModel = _projectService.CreateDetailsProjectViewModel(projectId);
            return View(viewModel);
        }

        // GET: Projects/Edit/5
        public async Task<ActionResult> Edit(Guid projectId)
        {
            var authResult = await _authService.AuthorizeAsync(HttpContext.User, projectId, "ProjectEditPrivileges");
            if (!authResult.Succeeded)
            {
                return new ForbidResult();
            }

            var viewModel = _projectService.CreateEditProjectViewModel(projectId);
            return View(viewModel);
        }

        // POST: Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProjectEditBindingModel form)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> errors = ModelState.Values.SelectMany(v => v.Errors);
                var viewModel = _projectService.CreateEditProjectViewModel(form.ProjectId, form, errors);
                return View(viewModel);
            }

            _projectService.EditProject(form);
            return RedirectToAction(nameof(Edit), new { projectId = form.ProjectId });
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
