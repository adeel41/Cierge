using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Cierge.Models;

namespace Cierge.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<AuthEvent> AuthEvents { get; set; }
        public DbSet<CustomClaim> CustomClaims { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasIndex(nameof(ApplicationUser.Email))
                .IsUnique(false);

            builder.Entity<AuthEvent>()
                .HasOne(e => e.User)
                .WithMany(u => u.Events)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CustomClaim>().ToTable("CustomClaims");

            var userCustomClaimEntity = builder.Entity<UserCustomClaim>().ToTable("UserCustomClaims");
            userCustomClaimEntity.HasKey(x => new {x.UserId, x.CustomClaimId});
            userCustomClaimEntity
                .HasOne(x => x.User)
                .WithMany(x => x.CustomClaims)
                .HasForeignKey(x => x.UserId);
            userCustomClaimEntity
                .HasOne(x => x.CustomClaim)
                .WithMany()
                .HasForeignKey(x => x.CustomClaimId);
        }
    }
}
