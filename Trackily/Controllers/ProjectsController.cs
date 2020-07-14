using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Binding;
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

        // GET: ProjectsController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ProjectsController/Details/5
        public ActionResult Details(int projectId)
        {
            return View();
        }

        // GET: ProjectsController/Create
        public ActionResult Create()
        {
            var viewModel = _projectService.CreateProjectViewModel();
            return View(viewModel);
        }

        // POST: ProjectsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BaseProjectBinding form)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> errors = ModelState.Values.SelectMany(v => v.Errors);
                var viewModel = _projectService.CreateProjectViewModel(form, errors);
                return View(viewModel);
            }

            _projectService.CreateProject(form);
        }

        // GET: ProjectsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProjectsController/Edit/5
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

        // GET: ProjectsController/Delete/5
        public ActionResult Delete(int projectId)
        {
            return View();
        }

        // POST: ProjectsController/Delete/5 
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
