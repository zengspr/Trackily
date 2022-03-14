using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Binding.Ticket;
using Trackily.Models.Domain;
using Trackily.Models.View.Ticket;

namespace Trackily.Services
{
    public class TicketService
    {
        private readonly UserManager<TrackilyUser> _userManager;
        private readonly UserTicketService _userTicketService;
        private readonly TrackilyContext _context;
        private readonly UserProjectService _userProjectService;

        public TicketService(UserManager<TrackilyUser> userManager,
                             TrackilyContext context,
                             UserTicketService userTicketService,
                             UserProjectService userProjectService)
        {
            _userManager = userManager;
            _userTicketService = userTicketService;
            _context = context;
            _userProjectService = userProjectService;
        }

        public List<TicketIndexViewModel> CreateIndexViewModel(IEnumerable<Ticket> selectedTickets)
        {
            var viewModels = new List<TicketIndexViewModel>();
            foreach (var ticket in selectedTickets)
            {
                viewModels.Add(new TicketIndexViewModel
                {
                    CreatorId = ticket.Creator.Id,
                    TicketId = ticket.TicketId,
                    CreatorName = $"{ticket.Creator.FirstName} {ticket.Creator.LastName}",
                    Title = ticket.Title,
                    ProjectTitle = ticket.Project.Title,
                    Priority = ticket.Priority,
                    Type = ticket.Type,
                    Status = ticket.Status,
                    NumAssignedUsers = ticket.Assigned.Count,
                    CreatedDate = ticket.CreatedDate,
                    UpdatedDate = ticket.UpdatedDate,
                });
            }

            return viewModels;
        }

        public async Task CreateTicket(TicketCreateBindingModel form, HttpContext request)
        {
            var ticket = new Ticket
            {
                Title = form.Title,
                Creator = await _userManager.GetUserAsync(request.User),
                Content = form.Content,
                CommentThreads = new List<CommentThread>(),
                Assigned = new List<UserTicket>(),
                Project = _context.Projects.Include(p => p.Members).Single(p => p.Title.Equals(form.SelectedProject))
            };

            var projectMemberIds = ticket.Project.Members.Select(m => m.Id).ToList();

            foreach (string username in form.AddAssigned.Where(entry => entry != null))
            {
                var user = _context.Users.Single(u => u.UserName == username);
                Debug.Assert(user != null);

                var userTicket = _userTicketService.CreateUserTicket(user, ticket);
                ticket.Assigned.Add(userTicket);

                // If the user is not already a member of the given project, add them as a member.
                if (!projectMemberIds.Contains(user.Id))
                {
                    var userProject = _userProjectService.CreateUserProject(user, ticket.Project);
                    ticket.Project.Members.Add(userProject);
                }
            }

            _context.Tickets.Add(ticket);
            _context.SaveChanges(true);
        }

        public async Task<TicketCreateViewModel> CreateTicketViewModel(
            HttpContext request, TicketCreateBindingModel ticket = null, IEnumerable<ModelError> errors = null)
        {
            var viewModel = new TicketCreateViewModel
            {
                Errors = new List<string>()
            };

            var currentUser = await _userManager.GetUserAsync(request?.User);
            var projectTitles = _context.Projects
                .Where(p => p.Members.Select(m => m.User).Contains(currentUser))
                .Select(p => p.Title)
                .ToList();
            viewModel.Projects = new SelectList(projectTitles);

            if (ticket != null)
            {
                viewModel.Title = ticket.Title;
                viewModel.Type = ticket.Type;
                viewModel.Priority = ticket.Priority;
            }

            if (errors != null)
            {
                foreach (var error in errors)
                {
                    viewModel.Errors.Add(error.ErrorMessage);
                }
            }

            return viewModel;
        }

        public TicketDetailsViewModel DetailsTicketViewModel(Ticket ticket, IEnumerable<ModelError> allErrors = null)
        {
            var viewModel = new TicketDetailsViewModel
            {
                CreatorId = ticket.Creator.Id,
                TicketId = ticket.TicketId,
                Title = ticket.Title,
                CreatedDate = ticket.CreatedDate,
                UpdatedDate = ticket.UpdatedDate,
                CreatorName = $"{ticket.Creator.FirstName} {ticket.Creator.LastName}",
                Assigned = _userTicketService.UserTicketToNames(ticket.Assigned),
                Type = ticket.Type,
                Status = ticket.Status,
                Priority = ticket.Priority,
                Content = ticket.Content,
                ProjectTitle = ticket.Project.Title
            };

            if (ticket.CommentThreads != null)
            {
                viewModel.CommentThreads = new List<CommentThread>();
                foreach (var commentThread in ticket.CommentThreads)
                {
                    viewModel.CommentThreads.Add(commentThread);
                }
            }

            if (allErrors != null)
            {
                viewModel.Errors = new List<string>();
                foreach (var error in allErrors)
                {
                    viewModel.Errors.Add(error.ErrorMessage);
                }
            }

            return viewModel;
        }

        public TicketEditViewModel EditTicketViewModel(Ticket ticket)
        {
            var viewModel = new TicketEditViewModel
            {
                TicketId = ticket.TicketId,
                Title = ticket.Title,
                CreatedDate = ticket.CreatedDate,
                UpdatedDate = ticket.UpdatedDate,
                CreatorName = $"{ticket.Creator.FirstName} {ticket.Creator.LastName}",
                Type = ticket.Type,
                Status = ticket.Status,
                Priority = ticket.Priority,
                Content = ticket.Content,
                ProjectTitle = ticket.Project.Title,
                RemoveAssigned = new Dictionary<string, bool>()
            };

            foreach (string username in _userTicketService.UserTicketToNames(ticket.Assigned))
            {
                viewModel.RemoveAssigned.Add(username, false);
            }

            return viewModel;
        }

        public TicketEditViewModel EditTicketViewModel(TicketEditBindingModel invalidInput, IEnumerable<ModelError> errors)
        {
            var viewModel = new TicketEditViewModel
            {
                TicketId = invalidInput.TicketId,
                Title = invalidInput.Title,
                CreatedDate = invalidInput.CreatedDate,
                UpdatedDate = invalidInput.UpdatedDate,
                CreatorName = invalidInput.CreatorName,
                Type = invalidInput.Type,
                Status = invalidInput.Status,
                Priority = invalidInput.Priority,
                Content = invalidInput.Content,
                RemoveAssigned = new Dictionary<string, bool>(),
                Errors = new List<string>()
            };

            if (invalidInput.RemoveAssigned != null)
            {
                foreach (var (username, flag) in invalidInput.RemoveAssigned)
                {
                    viewModel.RemoveAssigned[username] = flag;
                }
            }

            if (errors != null)
            {
                foreach (var error in errors)
                {
                    viewModel.Errors.Add(error.ErrorMessage);
                }
            }
            return viewModel;
        }

        public async Task EditTicket(Ticket ticket, TicketEditBindingModel input, HttpContext request)
        {
            var currentUser = await _userManager.GetUserAsync(request.User);

            ticket.UpdatedDate = DateTime.Now;
            ticket.Type = input.Type;
            ticket.Status = input.Status;
            ticket.Priority = input.Priority;
            ticket.Title = input.Title;
            ticket.Creator = currentUser;
            ticket.Content = input.Content;

            if (input.RemoveAssigned != null)
            {
                // Unassign the flagged users from the Ticket. 
                foreach (var (username, remove) in input.RemoveAssigned
                                                                  .Where(entry => entry.Value == true))
                {
                    var userId = _context.Users.Single(u => u.UserName == username).Id;
                    var userTicket = _context.UserTickets.Find(userId, ticket.TicketId);
                    ticket.Assigned.Remove(userTicket);
                    _context.UserTickets.Remove(userTicket);
                }
            }

            // Assign the users to the ticket by creating new UserTickets.
            foreach (string username in input.AddAssigned.Where(entry => entry != null))
            {
                var user = _context.Users.Single(u => u.UserName == username);
                var userTicket = _userTicketService.CreateUserTicket(user, ticket);
                ticket.Assigned.Add(userTicket);
            }
        }

        public Ticket GetTicket(Guid ticketId)
        {
            if (_context.Tickets.Find(ticketId) == null)
            {
                return null;
            }

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
