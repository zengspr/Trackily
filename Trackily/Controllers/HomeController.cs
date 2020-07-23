using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Domain;
using Trackily.Models.View;
using Trackily.Models.View.Home;
using Trackily.Services.Business;
using Trackily.Services.DataAccess;

namespace Trackily.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<TrackilyUser> _userManager;
        private readonly SignInManager<TrackilyUser> _signInManager;
        private readonly ProjectService _projectService;

        public HomeController(
            UserManager<TrackilyUser> userManager,
            SignInManager<TrackilyUser> signInManager,
            ProjectService projectService,
            TrackilyContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _projectService = projectService;
        }

        public async Task<IActionResult> Index()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            Debug.Assert(user != null);

            List<Project> projects = _projectService.GetProjectsForUserId(user.Id);

            var viewModels = new List<HomeIndexViewModel>();
            foreach (var project in projects)
            {
                var viewModel = new HomeIndexViewModel
                {
                    ProjectTitle = project.Title,
                    Tickets = new List<Ticket>()
                };
                var recentTickets = project.Tickets.Where(t => (DateTime.Now - t.CreatedDate).TotalHours <= 10);
                viewModel.Tickets.AddRange(recentTickets);

                viewModels.Add(viewModel);
            }

            return View(viewModels);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int id)
        {
            if (id == 404)
            {
                return View("Error404");
            }

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult PageNotAdded()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public async Task<IActionResult> LoginDevGuest()
        {
            await _signInManager.PasswordSignInAsync(DbSeeder.DevGuestName, DbSeeder.DevGuestPassword, true, false);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> LoginManGuest()
        {
            await _signInManager.PasswordSignInAsync(DbSeeder.ManagerGuestName, DbSeeder.ManagerGuestPassword, true, false);
            return RedirectToAction("Index");
        }
    }
}
