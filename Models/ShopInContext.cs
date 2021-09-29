using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ShopIn_API.Models;
using ShopIn_API.ViewModel;

#nullable disable

namespace ShopIn_API.Models
{
    public partial class ShopInContext : IdentityDbContext<ApplicationUser>
    {
        public ShopInContext()
        {
        }

        public ShopInContext(DbContextOptions<ShopInContext> options)
            : base(options)
        {

        }
        
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<ProductRate> ProductRates { get; set; }
        public virtual DbSet<PromoCode> PromoCodes { get; set; }
        public virtual DbSet<ApplicationUserPromoCode> ApplicationUserPromoCodes { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=.;Database=ShopIn;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasAnnotation("Relational:Collation", "Arabic_CI_AS");
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUserPromoCode>()
                .HasKey(pc => new { pc.PromoCodeId, pc.ApplicationUserId });
        }


    }
}
