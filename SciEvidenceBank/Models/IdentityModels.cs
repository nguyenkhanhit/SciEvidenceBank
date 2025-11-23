using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SciEvidenceBank.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class.
    public class ApplicationUser : IdentityUser
    {
        // navigation to user interests
        public virtual ICollection<UserInterest> UserInterests { get; set; } = new List<UserInterest>();
        // navigation to user interests (now referencing Categories)
      
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // authenticationType must match CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ResearchField
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<UserInterest> UserInterests { get; set; } = new List<UserInterest>();
    }

    public class UserInterest
    {
        public string UserId { get; set; }
        public int CategoryId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Category Category { get; set; }
    }

    // ApplicationDbContext must inherit IdentityDbContext<ApplicationUser>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // Uses connection string named "DefaultConnection" from Web.config
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        // OWIN factory used by Identity code: app.CreatePerOwinContext(ApplicationDbContext.Create)
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        // new sets
        public DbSet<ResearchField> ResearchFields { get; set; }
        public DbSet<UserInterest> UserInterests { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Evidence> Evidences { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<EvidenceTag> EvidenceTags { get; set; }
        public DbSet<MyCitation> MyCitations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // important for Identity table mappings
                                                // EvidenceTag composite key
            modelBuilder.Entity<EvidenceTag>()
                .HasKey(et => new { et.EvidenceId, et.TagId });

            modelBuilder.Entity<EvidenceTag>()
                .HasRequired(et => et.Evidence)
                .WithMany(e => e.EvidenceTags)
                .HasForeignKey(et => et.EvidenceId);

            modelBuilder.Entity<EvidenceTag>()
                .HasRequired(et => et.Tag)
                .WithMany(t => t.EvidenceTags)
                .HasForeignKey(et => et.TagId);

            // UserInterest composite key and relationships (User <-> Category)
            modelBuilder.Entity<UserInterest>()
                .HasKey(ui => new { ui.UserId, ui.CategoryId });

            modelBuilder.Entity<UserInterest>()
                .HasRequired(ui => ui.User)
                .WithMany(u => u.UserInterests)
                .HasForeignKey(ui => ui.UserId);

            modelBuilder.Entity<UserInterest>()
                .HasRequired(ui => ui.Category)
                .WithMany() // Categories may have other navigations; keep simple
                .HasForeignKey(ui => ui.CategoryId)
                .WillCascadeOnDelete(true);
        }
    }
}