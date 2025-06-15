using Microsoft.EntityFrameworkCore;

namespace Mini_Account_Management_System.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Screen> Screens { get; set; }
        public DbSet<UserRolePermission> UserRolePermissions { get; set; }
      

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User-Role relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            // Account self-referencing relationship
          
            // UserRolePermission relationships with delete behavior restrictions
            modelBuilder.Entity<UserRolePermission>()
                .HasOne(urp => urp.User)
                .WithMany()
                .HasForeignKey(urp => urp.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete issues

            modelBuilder.Entity<UserRolePermission>()
                .HasOne(urp => urp.Role)
                .WithMany(r => r.UserRolePermissions)
                .HasForeignKey(urp => urp.RoleId)
                .OnDelete(DeleteBehavior.Cascade); // Allow cascade deletion

            modelBuilder.Entity<UserRolePermission>()
                .HasOne(urp => urp.Screen)
                .WithMany(s => s.UserRolePermissions)
                .HasForeignKey(urp => urp.ScreenId)
                .OnDelete(DeleteBehavior.Cascade); // Allow cascade deletion

            // Seed data remains unchanged
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, RoleName = "Admin" },
                new Role { Id = 2, RoleName = "Accountant" },
                new Role { Id = 3, RoleName = "User" }
            );

            modelBuilder.Entity<Screen>().HasData(
                new Screen { Id = 1, ScreenName = "Dashboard", URL = "/" },
                new Screen { Id = 2, ScreenName = "Payment Voucher", URL = "/Vouchers/Payment" },
                new Screen { Id = 3, ScreenName = "Receipt Voucher", URL = "/Vouchers/Receipt" },
                new Screen { Id = 4, ScreenName = "Journal Entries", URL = "/Journal" },
                new Screen { Id = 5, ScreenName = "Chart of Accounts", URL = "/Accounts" },
                new Screen { Id = 6, ScreenName = "User Management", URL = "/Users" }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
