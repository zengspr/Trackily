using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class ProjectsController : Controller
    {
        private readonly ProjectService _projectService;

        public ProjectsController(ProjectService projectService)
        {
            _projectService = projectService;
        }

        // GET: Projects
        public ActionResult Index()
        {
            List<IndexProjectViewModel> projects = _projectService.CreateIndexProjectViewModels();
            return View(projects);
        }

        // GET: Projects/Details/5
        public ActionResult Details(int projectId)
        {
            return View();
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
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int projectId, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int projectId)
        {
            return View();
        }

        // POST: Projects/Delete/5 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int projectId, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
