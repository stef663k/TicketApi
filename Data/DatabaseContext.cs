using Microsoft.EntityFrameworkCore;
using TicketApi.Models;

namespace TicketApi.Data;
public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) 
    : base(options)
    {

    }

    public DbSet<Ticket> Tickets {get; set;}
    public DbSet<User> Users {get; set;}
    public DbSet<Notification> Notifications {get; set;}
    public DbSet<Comment> Comments {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User
        modelBuilder.Entity<User>()
            .HasKey(u => u.UserId);
            
        modelBuilder.Entity<User>()
            .Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);
            
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        // Ticket
        modelBuilder.Entity<Ticket>()
            .HasKey(t => t.TickedId);
        
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.AssignedUser)
            .WithMany(u => u.AssignedTickets)
            .HasForeignKey(t => t.AssignedUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.User)
            .WithMany(u => u.CreatedTickets)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Ticket>()
            .Property(t => t.Status)
            .HasConversion<string>();
            
        modelBuilder.Entity<Ticket>()
            .Property(t => t.Priority)
            .HasConversion<string>();
            
        modelBuilder.Entity<Ticket>()
            .Property(t => t.Category)
            .HasConversion<string>();

        // Comment
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Ticket)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Notification
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Notification>()
            .Property(n => n.Type)
            .HasConversion<string>();
    }

}