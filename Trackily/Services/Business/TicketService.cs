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
using Trackily.Data;
using Trackily.Models.Binding;
using Trackily.Models.Domain;
using Trackily.Models.View;
using Trackily.Services.Business;
using Trackily.Services.DataAccess;

namespace Trackily.Models.Services
{
    public class TicketService // Responsible for CRUD operations on Tickets. 
    {
        private readonly UserService _userService;
        private readonly UserTicketService _userTicketService;
        private readonly DbService _dbService;
        private readonly TrackilyContext _context;

        public TicketService(UserService userService, 
                             DbService dbService, 
                             TrackilyContext context, 
                             UserTicketService userTicketService)
        {
            _userService = userService;
            _userTicketService = userTicketService;
            _dbService = dbService;
            _context = context;
        }

         public async Task CreateTicket(CreateTicketBinding input, HttpContext request)
        {
            var ticket = new Ticket();
            ticket.Title = input.Title;
            ticket.Creator = await _userService.GetUser(request);
            ticket.Content = input.Content;

            string[] usernames = input.AddAssigned;
            foreach (string username in usernames)
            {
                if (username != null)
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
            }

            _context.Add(ticket);
            await _context.SaveChangesAsync();
        }

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

        // Creates a view model for the Edit ticket page using a ticket queried from the database.
        public async Task<EditTicketViewModel> EditTicketViewModel(Ticket ticket, 
                                                                   string flagUser = null,
                                                                   IEnumerable<ModelError> errors = null)
        {
            var viewModel = new EditTicketViewModel
            {
                Title = ticket.Title,
                CreatedDate = (DateTime)ticket.CreatedDate,
                UpdatedDate = (DateTime)ticket.UpdatedDate,
                CreatorUserName = await _dbService.GetCreatorUserName(ticket),
                IsApproved = ticket.IsApproved,
                IsReviewed = ticket.IsReviewed,
                Type = ticket.Type,
                Status = ticket.Status,
                Priority = ticket.Priority,
                Assigned = _userTicketService.UserTicketToNames(ticket.Assigned),
                Content = ticket.Content,
                RemoveAssigned = new Dictionary<string, bool>(),
                Errors = new List<string>()
            };

            foreach (string username in viewModel.Assigned)
            {
                if (username == flagUser)
                {
                    viewModel.RemoveAssigned[username] = true;
                }
                else
                {
                    viewModel.RemoveAssigned[username] = false;
                }
            }

            if (errors != null) // Add the errors in the binding model to the view model.
            {
                foreach (var error in errors)
                {
                    viewModel.Errors.Add(error.ErrorMessage);
                }
            }
            return viewModel;
        }

        public async Task EditTicket(Ticket ticket, EditTicketBinding input)
        {
            ticket.UpdatedDate = DateTime.Now;
            ticket.IsApproved = input.IsApproved;
            ticket.IsReviewed = input.IsReviewed;
            ticket.Type = input.Type;
            ticket.Status = input.Status;
            ticket.Priority = input.Priority;
            ticket.Title = input.Title;

            if (input.RemoveAssigned != null)
            {
                foreach (KeyValuePair<string, bool> entry in input.RemoveAssigned)
                {
                    if (entry.Value == true)
                    {
                        var userId = await _dbService.GetKey(entry.Key);
                        var userTicket = await _userTicketService.GetUserTicket(ticket.TicketId, userId);
                        ticket.Assigned.Remove(userTicket);
                        _context.UserTickets.Remove(userTicket);
                    }
                }
            }

            foreach (string newAssign in input.AddAssigned)
            {
                if (newAssign != null)
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
            }
        }
    }
}
