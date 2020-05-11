using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Domain;

namespace Trackily.Data
{
    public class TrackilyContext : IdentityDbContext<TrackilyUser, IdentityRole<Guid>, Guid>
    {
        public TrackilyContext(DbContextOptions<TrackilyContext> options)
            : base(options)
        {
        }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<UserTicket> UserTickets { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Manually define one-to-many relationship between User : Ticket to prevent the relationship
            // being optional. This ensures that Identity performs cascading deletion of Tickets.
            builder.Entity<TrackilyUser>()
                .HasMany(c => c.CreatedTicket)
                .WithOne(e => e.Creator)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction); // Prevent deletion of User if they have created Tickets.

            // Define composite primary key for User : Ticket relationship. 
            builder.Entity<UserTicket>()
                        .HasKey(createKey => new { createKey.Id, createKey.TicketId });

            // Manually define many-to-many relationship due to issue with conventions: TrackilyUser PK is called
            // Id instead of UserId due to derivation from IdentityUser. Changing TrackilyUser PK causes issues
            // with UserManager generate token method.
            // Deletion behaviour:
            //      - If a Ticket is deleted, all UserTickets with that TicketId should be deleted.
            //      - If a User is deleted, all UserTickets with that UserId should be deleted.
            builder.Entity<UserTicket>()
                        .HasOne(pt => pt.User)
                        .WithMany(p => p.Assigned)
                        .HasForeignKey(pt => pt.Id)
                        .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserTicket>()
                        .HasOne(pt => pt.Ticket)
                        .WithMany(t => t.Assigned)
                        .HasForeignKey(pt => pt.TicketId)
                        .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
