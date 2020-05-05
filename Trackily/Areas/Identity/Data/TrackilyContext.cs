using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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

            // Define composite primary key for User : Ticket relationship. 
            builder.Entity<UserTicket>()
                        .HasKey(createKey => new { createKey.Id, createKey.TicketId });

            // Manually define many-to-many relationship due to issue with convention. TrackilyUser PK is called
            // Id instead of UserId due to derivation from IdentityUser. Changing TrackilyUser PK causes issues
            // with UserManager generate token method.
            builder.Entity<UserTicket>()
                        .HasOne(pt => pt.User)
                        .WithMany(p => p.Assigned)
                        .HasForeignKey(pt => pt.Id);

            builder.Entity<UserTicket>()
                        .HasOne(pt => pt.Ticket)
                        .WithMany(t => t.Assigned)
                        .HasForeignKey(pt => pt.TicketId); 
        }
    }
}
