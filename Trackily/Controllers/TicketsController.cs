using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Trackily.Areas.Identity.Data;
using Trackily.Data;
using Trackily.Models.Binding;
using Trackily.Models.Domain;
using Trackily.Models.View;

namespace Trackily.Controllers
{
    public class TicketsController : Controller
    {
        private readonly TrackilyContext _context;
        private UserManager<TrackilyUser> _userManager;

        public TicketsController(TrackilyContext context, UserManager<TrackilyUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tickets.ToListAsync()); // List of Ticket objects - can get Id from them.
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.TicketId == id);
            if (ticket == null)
            {
                return NotFound();
            }

            var viewModel = new DetailsTicketViewModel
            {
                TicketId = ticket.TicketId,
                Title = ticket.Title,
                CreatedDate = (DateTime)ticket.CreatedDate,
                UpdatedDate = (DateTime)ticket.UpdatedDate,
                CreatorUserName = ticket.CreatorUserName,
                IsApproved = ticket.IsApproved,
                IsReviewed = ticket.IsReviewed,
                // TODO: Figure out how to display assigned usernames.
                Type = ticket.Type,
                Status = ticket.Status,
                Priority = ticket.Priority
            };

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
                var ticket = new Ticket();
                ticket.Title = Input.Title;
                // Set the user making the request as the Creator of the ticket.
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                ticket.Creator = currentUser;
                ticket.CreatorUserName = currentUser.UserName; // TODO: Convert to DI service? Create not satisfying SRP.

                // Create UserTicket objects and add them to the Ticket's Assigned property.
                // Assume that the provided usernames exist in the user database - validated by TicketBinding model.
                string[] usernames = Input.Assigned.Split(", "); // TODO: Make this less fragile - improve usability.
                foreach (string username in usernames) 
                {   
                    var user = await _context.Users.SingleAsync(u => u.UserName == username);
                    var assignUser = new UserTicket
                    {
                        Id = Guid.Parse(user.Id),
                        User = user,
                        TicketId = ticket.TicketId,
                        Ticket = ticket
                    };
                    ticket.Assigned.Add(assignUser);
                }

                _context.Add(ticket);
                await _context.SaveChangesAsync();
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

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // The Edit method receives a Ticket's Id as part of a route parameter.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Title,Assigned,Type,Priority,IsReviewed,IsApproved,Status")] EditTicketBinding Input)
        {
            // Need to update the given Ticket's properties but also the Assigned property on any Users who have been
            // assigned to the Ticket. See EF Core in Action Ch3. Also remember to update the UpdatedDate.
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Input);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(Input);
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

        private bool TicketExists(Guid id)
        {
            return _context.Tickets.Any(e => e.TicketId == id);
        }
    }
}
