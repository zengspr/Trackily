using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Areas.Identity.Data;
using Trackily.Controllers.Filters;
using Trackily.Models.Binding.Ticket;
using Trackily.Models.Domain;
using Trackily.Models.View.Ticket;
using Trackily.Services;


namespace Trackily.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly TrackilyContext _context;
        private readonly TicketService _ticketService;
        private readonly CommentService _commentService;
        private readonly IAuthorizationService _authService;
        private readonly UserManager<TrackilyUser> _userManager;

        public TicketsController(
            TrackilyContext context,
            TicketService ticketService,
            CommentService commentService,
            IAuthorizationService authService,
            UserManager<TrackilyUser> userManager)
        {
            _context = context;
            _ticketService = ticketService;
            _commentService = commentService;
            _authService = authService;
            _userManager = userManager;
        }

        // GET: Tickets
        public async Task<IActionResult> Index(string scope)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            List<Ticket> tickets;

            var query = _context.Tickets.Include(t => t.Assigned)
                                                                        .Include(t => t.Project);
            switch (scope)
            {
                case "created":
                    tickets = query.Where(t => t.Project.Members.Select(up => up.User).Contains(currentUser) &
                                               t.Creator == currentUser)
                                    .ToList();
                    break;
                case "assigned":
                    tickets = query.Include(t => t.Creator)
                                    .Where(t => t.Project.Members.Select(up => up.User).Contains(currentUser) &
                                                t.Assigned.Select(ut => ut.User).Contains(currentUser))
                                    .ToList();
                    break;
                case "closed":
                    tickets = query.Include(t => t.Creator).Where(t => t.Project.Members.Select(up => up.User).Contains(currentUser) &
                                                                       t.Status == Ticket.TicketStatus.Closed)
                                                            .ToList();
                    break;
                default: // Get all tickets.
                    tickets = query.Include(t => t.Creator)
                                    .Where(t => t.Project.Members.Select(up => up.User).Contains(currentUser) &
                                                t.Status != Ticket.TicketStatus.Closed)
                                    .ToList();
                    break;
            }

            List<TicketIndexViewModel> indexViewModel = _ticketService.CreateIndexViewModel(tickets);
            ViewData["indexScope"] = scope;
            return View(indexViewModel);
        }

        // GET: Tickets/Details/5
        [NullIdActionFilter]
        public IActionResult Details(Guid id)
        {
            var ticket = GetTicket(id);
            if (ticket == null)
            {
                return NotFound();
            }

            var viewModel = _ticketService.DetailsTicketViewModel(ticket);
            return View(viewModel);
        }

        // POST: Tickets/Details/5  
        [HttpPost]
        [NullIdActionFilter]
        public async Task<IActionResult> Details(Guid id, TicketDetailsBindingModel input)
        {
            var ticket = GetTicket(id);
            if (ticket == null)
            { return NotFound(); }

            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                var viewModel = _ticketService.DetailsTicketViewModel(ticket, allErrors);
                return View(viewModel);
            }

            if (input.CommentThreadContent != null)
            {
                await _commentService.AddCommentThread(ticket, input, HttpContext);
            }
            if (input.NewReplies != null)
            {
                await _commentService.AddComments(input, HttpContext);
            }

            ticket.UpdatedDate = DateTime.Now;
            _context.SaveChanges(true);

            return RedirectToAction("Details", new { id });
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            if (!_context.Projects.Any())
            {
                return RedirectToAction("Create", "Projects", new { redirected = true });
            }

            var viewModel = _ticketService.CreateTicketViewModel();
            return View(viewModel);
        }

        // POST: Tickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TicketCreateBindingModel input)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                var viewModel = _ticketService.CreateTicketViewModel(input, allErrors);
                return View(viewModel);
            }

            await _ticketService.CreateTicket(input, HttpContext);
            return RedirectToAction("Index", new { scope = "created" });
        }

        // GET: Tickets/Edit/5
        [NullIdActionFilter]
        public IActionResult Edit(Guid id)
        {
            var ticket = GetTicket(id);
            if (ticket == null)
            { return NotFound(); }

            var viewModel = _ticketService.EditTicketViewModel(ticket);
            return View(viewModel);
        }

        // POST: Tickets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NullIdActionFilter]
        public async Task<IActionResult> Edit(Guid id, TicketEditBindingModel input)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(m => m.Errors);
                var viewModel = _ticketService.EditTicketViewModel(input, allErrors);
                return View(viewModel);
            }

            var ticket = GetTicket(id);
            if (ticket == null)
            {
                return NotFound();
            }

            var authResult = await _authService.AuthorizeAsync(HttpContext.User, ticket.Creator.Id, "TicketEditPrivileges");
            if (!authResult.Succeeded)
            {
                return new ForbidResult();
            }

            await _ticketService.EditTicket(ticket, input, HttpContext);
            _context.SaveChanges(true);

            return ticket.Status == Ticket.TicketStatus.Closed ? RedirectToAction("Index", new { scope = "closed" }) : RedirectToAction("Index");
        }

        [NullIdActionFilter]
        public IActionResult DeleteTicket(Guid id)
        {
            var ticket = _context.Tickets.Find(id);
            if (ticket == null)
            {
                return NotFound();
            }

            _context.Tickets.Remove(ticket);
            _context.SaveChanges(true);

            return RedirectToAction("Index");
        }

        [NullIdActionFilter]
        public IActionResult DeleteCommentThread(Guid id, Guid ticketId)
        {
            var commentThread = _context.CommentThreads.Find(id);
            if (commentThread == null)
            {
                return NotFound();
            }

            _context.CommentThreads.Remove(commentThread);
            _context.SaveChanges(true);
            return RedirectToAction("Details", new { id = ticketId });
        }

        public IActionResult DeleteComment(Guid id, Guid ticketId)
        {
            var comment = _context.Comments.Find(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            _context.SaveChanges(true);

            return RedirectToAction("Details", new { id = ticketId });
        }

        private Ticket GetTicket(Guid ticketId)
        {
            var ticket = _context.Tickets.Include(t => t.Creator)
                .Include(t => t.Assigned)
                .ThenInclude(a => a.User)
                .Include(t => t.CommentThreads)
                .ThenInclude(ct => ct.Comments)
                .ThenInclude(c => c.Creator)
                .Include(t => t.CommentThreads)
                .ThenInclude(ct => ct.Creator)
                .Include(t => t.Project)
                .Single(t => t.TicketId == ticketId);
            return ticket;
        }
    }
}
