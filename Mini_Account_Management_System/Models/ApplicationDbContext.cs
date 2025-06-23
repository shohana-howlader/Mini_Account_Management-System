using Microsoft.EntityFrameworkCore;
using Mini_Account_Management_System.Models.ViewModel;

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
        public DbSet<UserRoleMapping> UserRoleMappings { get; set; }

        // ViewModel DbSets for stored procedures (keyless entities)
        public DbSet<UserRoleMappingViewModel> UserRoleMappingViews { get; set; }
        public DbSet<UserRolePermissionViewModel> UserRolePermission { get; set; }  // Add this line




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UserRolePermissionViewModel>().HasNoKey();

            modelBuilder.Entity<UserRoleMappingViewModel>()
                .HasNoKey()
                .ToView(null); // Not mapped to any table/view

            // Account self-referencing relationship

            // UserRolePermission relationships with delete behavior restrictions
            modelBuilder.Entity<UserRolePermission>()
                .HasOne(urp => urp.User)
                .WithMany()
                .HasForeignKey(urp => urp.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete issues

            //modelBuilder.Entity<UserRolePermission>()
            //    .HasOne(urp => urp.Role)
            //    .WithMany(r => r.UserRolePermissions)
            //    .HasForeignKey(urp => urp.RoleId)
            //    .OnDelete(DeleteBehavior.Cascade); // Allow cascade deletion

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


            modelBuilder.Entity<UserRoleMapping>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.RoleId).IsRequired();
                

                // Configure relationships
                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<Role>()
                      .WithMany()
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Ensure unique user-role combinations
                entity.HasIndex(e => new { e.UserId, e.RoleId })
                      .IsUnique()
                      .HasDatabaseName("IX_UserRoleMapping_UserId_RoleId");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
