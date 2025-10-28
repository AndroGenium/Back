using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class Baza : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // This method is ONLY for configuring the database connection
            // Don’t touch modelBuilder stuff here.
            optionsBuilder.UseSqlServer(
                @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ToolsBackend;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"
                );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USER → PRODUCT (listed)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Lender)
                .WithMany(u => u.ListedProducts)
                .HasForeignKey(p => p.LenderId)
                .OnDelete(DeleteBehavior.Restrict);

            // USER → PRODUCT (borrowed)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Borrower)
                .WithMany(u => u.BorrowedProducts)
                .HasForeignKey(p => p.BorrowerId)
                .OnDelete(DeleteBehavior.Restrict);

            // USER ↔ PRODUCT (liked) many-to-many
            modelBuilder.Entity<User>()
                .HasMany(u => u.LikedProducts)
                .WithMany(p => p.LikedByUsers)
                .UsingEntity(j => j.ToTable("UserLikedProducts"));

            // USER → REVIEW (one-to-many)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // PRODUCT → REVIEW (one-to-many)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
