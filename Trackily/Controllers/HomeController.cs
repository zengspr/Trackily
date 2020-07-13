using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Trackily.Areas.Identity.Data;
using Trackily.Models;
using Trackily.Services.DataAccess;

namespace Trackily.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<TrackilyUser> _userManager;
        private readonly SignInManager<TrackilyUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, 
            UserManager<TrackilyUser> userManager, SignInManager<TrackilyUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
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
