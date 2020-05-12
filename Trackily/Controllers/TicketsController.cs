using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Trackily.Areas.Identity.Data;
using Trackily.Data;
using Trackily.Models.Binding;
using Trackily.Models.Domain;
using Trackily.Models.Services;
using Trackily.Models.View;
using Trackily.Services.Business;
using Trackily.Services.DataAccess;

namespace Trackily.Controllers
{
    public class TicketsController : Controller
    {
        private readonly TrackilyContext _context;
        private readonly TicketService _ticketService;
        private readonly DbService _dbService;
        private readonly UserService _userService;

        public TicketsController(TrackilyContext context,
                                 TicketService ticketService,
                                 DbService dbService,
                                 UserService userService)
        {
            _context = context;
            _ticketService = ticketService;
            _dbService = dbService;
            _userService = userService;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tickets.ToListAsync()); 
        }

        // GET: Tickets/Details/5
        // TODO: Improve details view.
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _dbService.GetTicket(id, "assigned");
            if (ticket == null)
            {
                return NotFound();
            }

            var viewModel = await _ticketService.DetailsTicketViewModel(ticket);
            return View(viewModel);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            var viewModel = _ticketService.CreateTicketViewModel();
            return View(viewModel);
        }

        // POST: Tickets/Create
        // Note: Must be logged in for Create to work!
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTicketBinding input)
        {
            if (ModelState.IsValid)
            {
                await _ticketService.CreateTicket(input, HttpContext);
                return RedirectToAction(nameof(Index));
            }

            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            var viewModel = _ticketService.CreateTicketViewModel(input, allErrors);
            return View(viewModel);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _dbService.GetTicket(id, "assigned");
            if (ticket == null)
            {
                return NotFound();
            }

            var viewModel = await _ticketService.EditTicketViewModel(ticket);
            return View(viewModel);
        }

        // POST: Tickets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditTicketBinding input)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var ticket = await _dbService.GetTicket(id, "assigned"); 
                if (ticket == null)
                {
                    return NotFound();
                }

                await _ticketService.EditTicket(ticket, input);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", new { id = id });
            }

            // Validation errors have occurred.
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            var errorTicket = await _dbService.GetTicket(id, "assigned");
            var viewModel = await _ticketService.EditTicketViewModel(ticket: errorTicket, errors: allErrors);

            return View(viewModel);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(m => m.TicketId == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
