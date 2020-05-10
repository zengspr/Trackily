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
        private readonly UserManager<TrackilyUser> _userManager;
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
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Assigned,Type,Priority")] CreateTicketBinding Input)
        {
            if (ModelState.IsValid)
            {
                await _ticketService.CreateTicket(Input, HttpContext);
                return RedirectToAction(nameof(Index));
            }
            return View(Input);
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
        // The Edit method receives a Ticket's Id as part of a route parameter.
        // TODO: Add behaviour with POSTed RemoveAssigned information to remove those users from Ticket.
        //       Also, behaviour for adding a new user to Assigned.
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
                var ticket = await _dbService.GetTicket(id); 
                if (ticket == null)
                {
                    return NotFound();
                }

                await _ticketService.EditTicket(ticket, input);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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

        // TODO: Update to show which usernames are causing issues. 
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> ValidateAssigned(Guid ticketId, string[] usernames)
        {
            var alreadyAssigned = await _userService.UsernameAlreadyAssigned(ticketId, usernames);
            if (alreadyAssigned)
            {
                return Json("Some users are already assigned to the Ticket.");
            }

            var notExists = await _userService.UsernameNotExists(usernames);
            if (notExists)
            {
                return Json("Some users do not exist.");
            }

            return Json(true);
        }
    }
}
