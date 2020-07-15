using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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

            builder.Entity<Project>()
                .HasMany(p => p.Tickets)
                .WithOne(t => t.Project)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Project>()
                .HasMany(p => p.Members)
                .WithOne(u => u.Project);

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

            // Many-to-many relationship definitions 

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

            builder.Entity<UserProject>()
                .HasKey(createKey => new {createKey.Id, createKey.ProjectId});

            builder.Entity<UserProject>()
                .HasOne(up => up.User)
                .WithMany(u => u.AssignedProjects)
                .HasForeignKey(up => up.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserProject>()
                .HasOne(up => up.Project)
                .WithMany(p => p.Members)
                .HasForeignKey(up => up.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
