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
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentThread> CommentThreads { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Note: To delete a user, they must have all their created tickets deleted first. 
            //       Also, have to manually delete their created threads and comments afterwards. 
            builder.Entity<TrackilyUser>()
                .HasMany(c => c.CreatedTickets)
                .WithOne(e => e.Creator)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<TrackilyUser>()
                .HasMany(c => c.CreatedThreads)
                .WithOne(e => e.Creator)
                .OnDelete(DeleteBehavior.NoAction); 

            builder.Entity<TrackilyUser>()
                .HasMany(c => c.CreatedComments)
                .WithOne(e => e.Creator)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Ticket>()
                .HasMany(t => t.CommentThreads)
                .WithOne(c => c.Parent)
                .IsRequired(); // Delete threads associated with the ticket.

            builder.Entity<CommentThread>()
                .HasMany(t => t.Comments)
                .WithOne(c => c.Parent)
                .IsRequired(); // Delete comments associated with the thread. 

            // Manually define many-to-many relationship due to issue with conventions: TrackilyUser PK is called
            // Id instead of UserId due to derivation from IdentityUser. Changing TrackilyUser PK causes issues
            // with UserManager generate token method.
            // Deletion behaviour:
            //      - If a Ticket is deleted, all UserTickets with that TicketId should be deleted.
            //      - If a User is deleted, all UserTickets with that UserId should be deleted.
            builder.Entity<UserTicket>()
                .HasKey(createKey => new { createKey.Id, createKey.TicketId });

            builder.Entity<UserTicket>()
                .HasOne(pt => pt.User)
                .WithMany(p => p.AssignedTo)
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
