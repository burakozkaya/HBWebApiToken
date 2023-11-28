using HBWebApiToken.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HBWebApiToken.Context;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
{
    public DbSet<Book> Books { get; set; }
    public DbSet<UserFavBook> FavBooks { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<UserFavBook>()
            .HasKey(x => new { x.AppUserId, x.BookId });

        var adminRoleId = Guid.NewGuid().ToString();
        builder.Entity<AppRole>().HasData(new AppRole { Id = Guid.NewGuid().ToString(), Name = "User" }, new AppRole { Id = adminRoleId, Name = "Admin" });

        var adminId = Guid.NewGuid().ToString();
        var hasher = new PasswordHasher<AppUser>();
        builder.Entity<AppUser>().HasData(new AppUser
        {
            Id = adminId, 
            UserName = "admin", 
            NormalizedUserName = "ADMIN", 
            Email = "admin@example.com", 
            NormalizedEmail = "ADMIN@EXAMPLE.COM", 
            EmailConfirmed = true, 
            PasswordHash = hasher.HashPassword(null, "admin"),
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(), 
            PhoneNumber = "1234567890",
            PhoneNumberConfirmed = true, 
            TwoFactorEnabled = false, 
            LockoutEnabled = false,
            AccessFailedCount = 0, 
            LockoutEnd = null, 
        });

        builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string> { RoleId = adminRoleId, UserId = adminId });
        base.OnModelCreating(builder);
    }
}
