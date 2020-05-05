using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public TicketService(UserService userService, DbService dbService, TrackilyContext context, UserTicketService userTicketService)
        {
            _userService = userService;
            _userTicketService = userTicketService;
            _dbService = dbService;
            _context = context;
        }

         public async Task CreateTicket(CreateTicketBinding input, HttpContext request)
            // TODO: Add exceptions / errors.
            // TODO: Check that assigning multiple users to a ticket works and that the users are referenced
            //       by UserId in the UserTicket object. 
        {
            var ticket = new Ticket();
            ticket.Title = input.Title;
            ticket.Creator = await _userService.GetUser(request);

            string[] usernames = input.Assigned.Split(", "); // TODO: Improve this. 
            foreach (string username in usernames)
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
                // TODO: Figure out how to display assigned usernames. Can get all user IDs associated with the 
                //       ticket using smth like ticket.Assigned.Where(t => TicketId == id) and then select all users
                //       whose id is in the collection, but seems inefficient. Could directly store usernames in the
                //       UserTicket entity?
                Type = ticket.Type,
                Status = ticket.Status,
                Priority = ticket.Priority
            };
            return viewModel;
        }

        // Creates a view model for the Edit ticket page using a ticket queried from the database.
        public async Task<EditTicketViewModel> EditTicketViewModel(Ticket ticket)
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
                Assigned = _userTicketService.UserTicketToNames(ticket.Assigned)
                // TODO: Edit view still only shows one assigned user, even though two were assigned during 
                //       ticket creation. Might be an issue with the Ticket Create method.
            };

            return viewModel;
        }

        public async Task EditTicket(Ticket ticket, EditTicketBinding Input)
        {
            // Two cases: new users are added to Assigned, or some existing users are removed from Assigned
            // (or both).
            ticket.UpdatedDate = DateTime.Now;
        }
    }
}
