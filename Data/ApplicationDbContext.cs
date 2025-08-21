using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SenegaleseAssociation.Models;

namespace SenegaleseAssociation.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Leadership> Leadership { get; set; }
        public DbSet<CommunityHighlight> CommunityHighlights { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Event configuration
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Time).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Location).HasMaxLength(200);
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.HasIndex(e => e.Date);
                entity.HasIndex(e => e.IsActive);
            });

            // Service configuration
            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Title).IsRequired().HasMaxLength(100);
                entity.Property(s => s.Description).IsRequired().HasMaxLength(500);
                entity.Property(s => s.IconClass).HasMaxLength(50);
                entity.HasIndex(s => s.DisplayOrder);
                entity.HasIndex(s => s.IsActive);
            });

            // Leadership configuration
            modelBuilder.Entity<Leadership>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.Property(l => l.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(l => l.LastName).IsRequired().HasMaxLength(100);
                entity.Property(l => l.Position).IsRequired().HasMaxLength(100);
                entity.Property(l => l.Bio).HasMaxLength(500);
                entity.Property(l => l.WelcomeMessage).HasMaxLength(2000);
                entity.Property(l => l.ImageUrl).HasMaxLength(200);
                entity.Property(l => l.Email).HasMaxLength(100);
                entity.HasIndex(l => l.IsPresident);
                entity.HasIndex(l => l.DisplayOrder);
                entity.HasIndex(l => l.IsActive);
            });

            // CommunityHighlight configuration
            modelBuilder.Entity<CommunityHighlight>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Title).IsRequired().HasMaxLength(200);
                entity.Property(c => c.Description).IsRequired().HasMaxLength(1000);
                entity.Property(c => c.IconClass).HasMaxLength(50);
                entity.Property(c => c.BackgroundClass).HasMaxLength(50);
                entity.Property(c => c.Stat1Number).HasMaxLength(20);
                entity.Property(c => c.Stat1Label).HasMaxLength(50);
                entity.Property(c => c.Stat2Number).HasMaxLength(20);
                entity.Property(c => c.Stat2Label).HasMaxLength(50);
                entity.Property(c => c.Stat3Number).HasMaxLength(20);
                entity.Property(c => c.Stat3Label).HasMaxLength(50);
                entity.HasIndex(c => c.DisplayOrder);
                entity.HasIndex(c => c.IsActive);
            });

            // Testimonial configuration
            modelBuilder.Entity<Testimonial>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.AuthorName).IsRequired().HasMaxLength(100);
                entity.Property(t => t.AuthorTitle).IsRequired().HasMaxLength(100);
                entity.Property(t => t.Content).IsRequired().HasMaxLength(1000);
                entity.Property(t => t.AuthorImageUrl).HasMaxLength(200);
                entity.HasIndex(t => t.IsFeatured);
                entity.HasIndex(t => t.IsActive);
            });

            // ContactMessage configuration
            modelBuilder.Entity<ContactMessage>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Email).IsRequired().HasMaxLength(200);
                entity.Property(c => c.Subject).IsRequired().HasMaxLength(200);
                entity.Property(c => c.Message).IsRequired().HasMaxLength(2000);
                entity.HasIndex(c => c.CreatedAt);
            });
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is ITimestamped && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                var timestamped = (ITimestamped)entity.Entity;
                
                if (entity.State == EntityState.Added)
                {
                    timestamped.CreatedAt = DateTime.UtcNow;
                }
                
                timestamped.UpdatedAt = DateTime.UtcNow;
            }
        }
    }

    public interface ITimestamped
    {
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
    }
}