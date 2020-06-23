using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Trackily.Areas.Identity.Data;
using Trackily.Controllers.Filters;
using Trackily.Data;
using Trackily.Models.Binding;
using Trackily.Models.Domain;
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
        private readonly UserManager<TrackilyUser> _userManager;

        public TicketsController(TrackilyContext context, 
            TicketService ticketService, DbService dbService, CommentService commentService,
            IAuthorizationService authService, UserManager<TrackilyUser> userManager)
        {
            _context = context;
            _ticketService = ticketService;
            _dbService = dbService;
            _commentService = commentService;
            _authService = authService;
            _userManager = userManager;
        }

        // GET: Tickets
        public async Task<IActionResult> Index(string scope)
        {
            List<Ticket> tickets;
            List<IndexViewModel> indexViewModel;
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            switch (scope)
            {
                case "created":
                    tickets = await _context.Tickets.Include(t => t.Assigned)
                                                    .Where(t => t.Creator == currentUser)
                                                    .ToListAsync();
                    break;
                case "assigned":
                    tickets = await _context.Tickets.Include(t => t.Assigned)
                                                    .Where(t => t.Assigned.Select(ut => ut.User).Contains(currentUser))
                                                    .ToListAsync();
                    break;
                case "closed":
                    tickets = await _context.Tickets.Include(t => t.Assigned)
                                            .Where(t => t.Status == Ticket.TicketStatus.Closed)
                                            .ToListAsync();
                    break;
                default: // Get all tickets.
                    tickets = await _context.Tickets.Include(t => t.Assigned)
                                                    .ToListAsync();
                    break;
            }

            indexViewModel = await _ticketService.CreateIndexViewModel(tickets);
            ViewData["indexScope"] = scope;
            return View(indexViewModel); 
        }

        // GET: Tickets/Details/5
        [NullIdActionFilter]
        public async Task<IActionResult> Details(Guid id)
        {
            var ticket = await _dbService.GetTicket(id);
            if (ticket == null)
            {
                return NotFound();
            }

            var viewModel = await _ticketService.DetailsTicketViewModel(ticket);
            return View(viewModel);
        }

        // POST: Tickets/Details/5  
        [HttpPost]
        [NullIdActionFilter]
        public async Task<IActionResult> Details(Guid id, DetailsTicketBinding input)
        {
            var ticket = await _dbService.GetTicket(id);
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
            return RedirectToAction("Details", new { id });
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
            return RedirectToAction("Index", new {scope = "created"});
        }

        // GET: Tickets/Edit/5
        [NullIdActionFilter]
        public async Task<IActionResult> Edit(Guid id)
        {
            var ticket = await _dbService.GetTicket(id);
            if (ticket == null) { return NotFound(); }

            var viewModel = await _ticketService.EditTicketViewModel(ticket);
            return View(viewModel);
        }

        // POST: Tickets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NullIdActionFilter]
        public async Task<IActionResult> Edit(Guid id, EditTicketBinding input)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(m => m.Errors);
                var viewModel = _ticketService.EditTicketViewModel(input, allErrors);
                return View(viewModel);
            }

            var ticket = await _dbService.GetTicket(id);
            if (ticket == null)
            {
                return NotFound();
            }

            var authResult = await _authService.AuthorizeAsync(HttpContext.User, ticket, "HasEditPrivileges");
            if (!authResult.Succeeded)
            {
                return new ForbidResult();
            }

            await _ticketService.EditTicket(ticket, input, HttpContext);
            await _context.SaveChangesAsync();

            return ticket.Status == Ticket.TicketStatus.Closed ? RedirectToAction("Index", new {scope = "closed"}) : RedirectToAction("Index");
        }

        [NullIdActionFilter]
        public async Task<IActionResult> DeleteTicket(Guid id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [NullIdActionFilter]
        public async Task<IActionResult> DeleteCommentThread(Guid id, Guid ticketId)
        {
            var commentThread = await _context.CommentThreads
                .FirstOrDefaultAsync(ct => ct.CommentThreadId == id);
            if (commentThread == null)
            {
                return NotFound();
            }

            _context.CommentThreads.Remove(commentThread);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = ticketId });
        }

        public async Task<IActionResult> DeleteComment(Guid id, Guid ticketId)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = ticketId });
        }
    }
}
