using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SciEvidenceBank.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // authenticationType must match CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
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
        }
    }
}