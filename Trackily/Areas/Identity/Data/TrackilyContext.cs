using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using Trackily.Models.Domain;

namespace Trackily.Areas.Identity.Data
{
    public class TrackilyContext : IdentityDbContext<TrackilyUser, IdentityRole<Guid>, Guid>
    {
        public TrackilyContext(DbContextOptions<TrackilyContext> options)
            : base(options) { }

        public DbSet<Project> Projects { get; set; }
        public DbSet<UserProject> UserProjects { get; set; }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<UserTicket> UserTickets { get; set; }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentThread> CommentThreads { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Users
            // One-to-many relationship between Users (Creator) and Tickets, CommentThread, and Comments.
            // Each of the entities on the right have only one Creator.
            builder.Entity<TrackilyUser>()
                .HasMany(c => c.CreatedTickets)
                .WithOne(e => e.Creator);

            builder.Entity<TrackilyUser>()
                .HasMany(c => c.CreatedThreads)
                .WithOne(e => e.Creator);

            builder.Entity<TrackilyUser>()
                .HasMany(c => c.CreatedComments)
                .WithOne(e => e.Creator);

            // Projects

            // One-to-many relationship between Projects and Tickets. 
            builder.Entity<Project>()
                .HasMany(p => p.Tickets)
                .WithOne(t => t.Project)
                .IsRequired(); // Deleting a project deletes all associated Tickets.

            // Many-to-many relationship between Users and Projects. 
            builder.Entity<UserProject>()
                .HasKey(key => new { key.Id, key.ProjectId });
            builder.Entity<UserProject>()
                .HasOne(up => up.User)
                .WithMany(u => u.AssignedProjects)
                .HasForeignKey(up => up.Id);
            builder.Entity<UserProject>()
                .HasOne(up => up.Project)
                .WithMany(p => p.Members)
                .HasForeignKey(up => up.ProjectId);

            // Tickets
            // One-to-many relationship between Tickets and CommentThreads.
            builder.Entity<Ticket>()
                .HasMany(t => t.CommentThreads)
                .WithOne(c => c.Parent)
                .IsRequired(); // Delete threads associated with the ticket.

            // Many-to-many relationship between Users and Tickets.
            builder.Entity<UserTicket>()
                .HasKey(createKey => new { createKey.Id, createKey.TicketId });
            builder.Entity<UserTicket>()
                .HasOne(pt => pt.User)
                .WithMany(p => p.AssignedTo)
                .HasForeignKey(pt => pt.Id);
            builder.Entity<UserTicket>()
                .HasOne(pt => pt.Ticket)
                .WithMany(t => t.Assigned)
                .HasForeignKey(pt => pt.TicketId)
                .OnDelete(DeleteBehavior.Cascade); 

            // One-to-many relationship between CommentThreads and Comments.
            builder.Entity<CommentThread>()
                .HasMany(t => t.Comments)
                .WithOne(c => c.Parent)
                .IsRequired(); // Delete comments associated with the thread. 
        }
    }
}
