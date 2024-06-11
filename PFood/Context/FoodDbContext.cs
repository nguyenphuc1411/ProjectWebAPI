using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using PFood.Data;

namespace PFood.Context
{
    public class FoodDbContext:IdentityDbContext<User>
    {
        public FoodDbContext(DbContextOptions<FoodDbContext> options) : base(options) { }


        #region DBSET
        public DbSet<Category> Categories { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Food>(entity =>
            {
                entity.HasOne(x => x.Category).WithMany(x => x.Foods)
                .HasForeignKey(x => x.CategoryID).OnDelete(DeleteBehavior.Cascade);
            });          
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(o => o.User)
               .WithMany(u => u.Orders)
               .HasForeignKey(o => o.UserID)
               .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Coupon).WithOne(x => x.Order)
                .HasForeignKey<Order>(x => x.CouponID).IsRequired(false).OnDelete(DeleteBehavior.Cascade); 

            });
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(x => new {x.OrderID, x.FoodID});
                entity.HasOne(x => x.Order).WithMany(x => x.OrderDetails)
                .HasForeignKey(x => x.OrderID).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x=>x.Food).WithMany(x=>x.OrderDetails)
                .HasForeignKey(x=>x.FoodID).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasOne(x => x.User).WithMany(x => x.Reviews)
                .HasForeignKey(x => x.UserID).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.Food).WithMany(x => x.Reviews)
               .HasForeignKey(x => x.FoodID).OnDelete(DeleteBehavior.Cascade); ;
            });
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasOne(x => x.Order).WithOne(x => x.Payment)
                .HasForeignKey<Payment>(x => x.OrderID).OnDelete(DeleteBehavior.Cascade); 
            });

            modelBuilder.Entity<Coupon>(entity =>
            {
                entity.HasIndex(x => x.CouponCode).IsUnique();
            });

            SeedRoles(modelBuilder);

        }
        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>(entity =>
            {
                entity.HasData(
                    new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
                    new IdentityRole { Name = "Customer", NormalizedName = "CUSTOMER" }
                );
            });
        }
    }
}
