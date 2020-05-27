using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Trackily.Areas.Identity.Data;
using Trackily.Data;
using Trackily.Models.Binding;
using Trackily.Models.Domain;
using Trackily.Models.View;
using Trackily.Services.Business;
using Trackily.Services.DataAccess;

namespace Trackily.Services.Business
{
    /// <summary>
    /// Contains methods which generate view and binding models related to Tickets.
    /// </summary>
    public class TicketService 
    {
        private readonly UserManager<TrackilyUser> _userManager;
        private readonly UserTicketService _userTicketService;
        private readonly DbService _dbService;
        private readonly TrackilyContext _context;

        public TicketService(UserManager<TrackilyUser> userManager, 
                             DbService dbService, 
                             TrackilyContext context, 
                             UserTicketService userTicketService)
        {
            _userManager = userManager;
            _userTicketService = userTicketService;
            _dbService = dbService;
            _context = context;
        }

        /// <summary>
        /// Creates a list containing view models for every Ticket currently in the database.
        /// </summary>
        /// <param name="allTickets">Collection containing all Ticket objects currently in the database. </param>
        /// <returns>A list of view models for each Ticket in the database.</returns>
        public List<IndexViewModel> CreateIndexViewModel(IEnumerable<Ticket> allTickets)
        {
            return allTickets.Select(ticket => new IndexViewModel
                {
                    TicketId = ticket.TicketId,
                    Title = ticket.Title,
                    Priority = ticket.Priority,
                    Type = ticket.Type,
                    Status = ticket.Status,
                    NumAssignedUsers = ticket.Assigned.Count,
                    CreatedDate = ticket.CreatedDate,
                    UpdatedDate = ticket.UpdatedDate,
                    IsReviewed = ticket.IsReviewed,
                    IsApproved = ticket.IsApproved
                })
                .ToList();
        }

        /// <summary>
        /// Creates a new Ticket object using the binding model and saves it to the database.
        /// </summary>
        /// <param name="input">Binding model for creating a new Ticket object.</param>
        /// <param name="request">The HttpContext of the current request.</param>
        /// <returns>N/A</returns>
        public async Task CreateTicket(CreateTicketBinding input, HttpContext request)
        {
            var ticket = new Ticket
            {
                Title = input.Title,
                Creator = await _userManager.GetUserAsync(request.User),
                Content = input.Content,
                CommentThreads = new List<CommentThread>(),
                Assigned = new List<UserTicket>()
            };

            foreach (string username in input.AddAssigned.Where(entry => entry != null))
            {
                var user = await _dbService.GetUser(username);
                var assignUser = new UserTicket
                {
                    Id = user.Id,
                    User = user,
                    TicketId = ticket.TicketId,
                    Ticket = ticket
                };
                ticket.Assigned.Add(assignUser);
            }

            _context.Add(ticket);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Creates a view model for the Tickets.Create view.
        /// </summary>
        /// <remarks>
        /// If a binding model and errors are provided, creates a view model displaying the errors.
        /// </remarks>
        /// <param name="ticket">Ticket binding model that failed validation.</param>
        /// <param name="errors">Collection of ModelError related to failed Ticket creation.</param>
        /// <returns>CreateTicketViewModel object</returns>
        public CreateTicketViewModel CreateTicketViewModel(CreateTicketBinding ticket = null,
                                                           IEnumerable<ModelError> errors = null)
        {
            var viewModel = new CreateTicketViewModel
            {
                Errors = new List<string>()
            };

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

        /// <summary>
        /// Creates a view model for the Tickets.Details view.
        /// </summary>
        /// <param name="ticket">Ticket object to create the Details view for.</param>
        /// <returns>DetailsTicketViewModel object</returns>
        public async Task<DetailsTicketViewModel> DetailsTicketViewModel(Ticket ticket)
        {
            var viewModel = new DetailsTicketViewModel
            {
                TicketId = ticket.TicketId,
                Title = ticket.Title,
                CreatedDate = (DateTime)ticket.CreatedDate,
                UpdatedDate = (DateTime)ticket.UpdatedDate,
                CreatorUserName = await _dbService.GetCreatorUserName(ticket),
                IsApproved = ticket.IsApproved,
                IsReviewed = ticket.IsReviewed,
                Assigned = _userTicketService.UserTicketToNames(ticket.Assigned),
                Type = ticket.Type,
                Status = ticket.Status,
                Priority = ticket.Priority
            };
            return viewModel;
        }

        /// <summary>
        /// Generates a Ticket view model for the Tickets.Edit view from the given ticket ID.
        /// </summary>
        /// <remarks>
        /// To generate view models for binding models with errors, see
        /// <see cref="TicketService.EditTicketViewModel(EditTicketBinding, IEnumerable{ModelError})"/>.
        /// </remarks>
        /// <param name="ticket">Ticket object to create the Edit view for.</param>
        /// <returns>EditTicketViewModel object.</returns>
        public async Task<EditTicketViewModel> EditTicketViewModel(Ticket ticket)
        {
            var viewModel = new EditTicketViewModel
            {
                TicketId = ticket.TicketId,  
                Title = ticket.Title,
                CreatedDate = ticket.CreatedDate,
                UpdatedDate = ticket.UpdatedDate,
                CreatorUserName = await _dbService.GetCreatorUserName(ticket),
                IsApproved = ticket.IsApproved,
                IsReviewed = ticket.IsReviewed,
                Type = ticket.Type,
                Status = ticket.Status,
                Priority = ticket.Priority,
                Content = ticket.Content,
                CommentThreads = new List<CommentThread>(),
                RemoveAssigned = new Dictionary<string, bool>(),
                Errors = new List<string>()
            };

            if (ticket.CommentThreads != null)
            {
                foreach (var commentThread in ticket.CommentThreads)
                {
                    viewModel.CommentThreads.Add(commentThread);
                }
            }
            foreach (string username in _userTicketService.UserTicketToNames(ticket.Assigned))
            {
                viewModel.RemoveAssigned.Add(username, false);
            }
            return viewModel;
        }

        /// <summary>
        /// Generates a Ticket view model for the Tickets.Edit view from the given binding model.
        /// </summary>
        /// <remarks>
        /// To generate view models for tickets retrieved from the database, see
        /// <see cref="TicketService.EditTicketViewModel(Ticket)"/>.
        /// </remarks>
        /// <param name="invalidInput">EditTicketBinding model with validation errors.</param>
        /// <param name="errors">Collection of ModelError for the invalid binding model.</param>
        /// <returns>EditTicketViewModel object.</returns>
        public EditTicketViewModel EditTicketViewModel(EditTicketBinding invalidInput, IEnumerable<ModelError> errors)
        {
            var viewModel = new EditTicketViewModel
            {
                TicketId = invalidInput.TicketId,
                Title = invalidInput.Title,
                CreatedDate = invalidInput.CreatedDate,
                UpdatedDate = invalidInput.UpdatedDate,
                CreatorUserName = invalidInput.CreatorUserName,
                IsApproved = invalidInput.IsApproved,
                IsReviewed = invalidInput.IsReviewed,
                Type = invalidInput.Type,
                Status = invalidInput.Status,
                Priority = invalidInput.Priority,
                Content = invalidInput.Content,
                CommentThreadContent = invalidInput.CommentThreadContent,
                CommentThreads = new List<CommentThread>(),
                RemoveAssigned = new Dictionary<string, bool>(),
                Errors = new List<string>()
            };

            foreach (var (username, flag) in invalidInput.RemoveAssigned)
            {
                viewModel.RemoveAssigned[username] = flag;
            }
            foreach (var error in errors)
            {
                viewModel.Errors.Add(error.ErrorMessage);
            }
            return viewModel;
        }

        /// <summary>
        /// Updates the properties of the Ticket using values from the input.
        /// </summary>
        /// <param name="ticket">Ticket object to be updated.</param>
        /// <param name="input">EditTicketBinding model containing POSTed values.</param>
        /// <param name="request"></param>
        /// <returns>N/A</returns>
        public async Task EditTicket(Ticket ticket, EditTicketBinding input, HttpContext request)
        {
            ticket.UpdatedDate = DateTime.Now;
            ticket.IsApproved = input.IsApproved;
            ticket.IsReviewed = input.IsReviewed;
            ticket.Type = input.Type;
            ticket.Status = input.Status;
            ticket.Priority = input.Priority;
            ticket.Title = input.Title;
            ticket.CommentThreads = null;

            if (input.RemoveAssigned != null)
            {
                // Unassign the flagged users from the Ticket. 
                foreach (KeyValuePair<string, bool> entry in input.RemoveAssigned
                                                                  .Where(entry => entry.Value == true))
                {
                    var userId = await _dbService.GetKey(entry.Key);
                    var userTicket = await _userTicketService.GetUserTicket(ticket.TicketId, userId);
                    ticket.Assigned.Remove(userTicket);
                    _context.UserTickets.Remove(userTicket);
                }
            }

            foreach (string newAssign in input.AddAssigned.Where(entry => entry != null)) // Assign the users to the ticket. 
            {
                var userTicket = new UserTicket
                {
                    Id = await _dbService.GetKey(newAssign),
                    TicketId = ticket.TicketId,
                    User = await _dbService.GetUser(newAssign),
                    Ticket = ticket
                };
                ticket.Assigned.Add(userTicket);
            }

            if (input.CommentThreadContent != null) // Create a new CommentThread.
            {
                ticket.CommentThreads = new List<CommentThread>();
                var commentThread = new CommentThread
                {
                    Parent = ticket,
                    Content = input.CommentThreadContent,
                    Creator = await _userManager.GetUserAsync(request.User)
                };
                ticket.CommentThreads.Add(commentThread);
            }
        }

    }
}
