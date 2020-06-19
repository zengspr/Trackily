using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Data;
using Trackily.Models.Binding;
using Trackily.Models.View;
using Trackily.Services.Business;
using Trackily.Services.DataAccess;

namespace Trackily.Controllers
{
    [Authorize] 
    public class TicketsController : Controller
    {
        private readonly TrackilyContext _context;
        private readonly TicketService _ticketService;
        private readonly DbService _dbService;
        private readonly CommentService _commentService;
        private readonly IAuthorizationService _authService;

        public TicketsController(TrackilyContext context, 
            TicketService ticketService, DbService dbService, CommentService commentService,
            IAuthorizationService authService)
        {
            _context = context;
            _ticketService = ticketService;
            _dbService = dbService;
            _commentService = commentService;
            _authService = authService;
        }

        // GET: Tickets

        public async Task<IActionResult> Index()
        {
            var allTickets = await _context.Tickets.Include(a => a.Assigned).ToListAsync();
            List<IndexViewModel> indexViewModel = await _ticketService.CreateIndexViewModel(allTickets);
            return View(indexViewModel); 
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _dbService.GetTicket(id.Value);
            if (ticket == null)
            {
                return NotFound();
            }

            var viewModel = await _ticketService.DetailsTicketViewModel(ticket);
            return View(viewModel);
        }

        // POST: Tickets/Details/5  
        [HttpPost]
        public async Task<IActionResult> Details(Guid? ticketId, DetailsTicketBinding input)
        {
            if (ticketId == null) { return NotFound(); }

            var ticket = await _dbService.GetTicket(ticketId.Value);
            if (ticket == null) { return NotFound(); }

            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                var viewModel = await _ticketService.DetailsTicketViewModel(ticket, allErrors);
                return View(viewModel);
            }

            if (input.CommentThreadContent != null)
            {
                await _commentService.AddCommentThread(ticket, input, HttpContext);
            }
            if (input.NewReplies != null)
            {
                await _commentService.AddComments(ticket, input, HttpContext);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = ticketId });
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
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                var viewModel = _ticketService.CreateTicketViewModel(input, allErrors);
                return View(viewModel);
            }

            await _ticketService.CreateTicket(input, HttpContext);
            return RedirectToAction(nameof(Index));
        }

        // GET: Tickets/Edit/5
        // TODO: Make (id == null) check into a validation attribute.
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) { return NotFound(); }

            var ticket = await _dbService.GetTicket(id.Value);
            if (ticket == null) { return NotFound(); }

            var viewModel = await _ticketService.EditTicketViewModel(ticket: ticket);
            return View(viewModel);
        }

        // POST: Tickets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid? id, EditTicketBinding input)
        {
            if (id == null) { return NotFound(); }

            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(m => m.Errors);
                var viewModel = _ticketService.EditTicketViewModel(input, allErrors);
                return View(viewModel);
            }

            var ticket = await _dbService.GetTicket(id.Value); 
            if (ticket == null) { return NotFound(); }

            var authResult = await _authService.AuthorizeAsync(HttpContext.User, ticket, "HasEditPrivileges");
            if (!authResult.Succeeded) { return new ForbidResult(); }

            await _ticketService.EditTicket(ticket, input, HttpContext);
            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", new { id = id.Value });
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
