using HBWebApiToken.Entity;
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
        base.OnModelCreating(builder);
        builder.Entity<UserFavBook>()
            .HasKey(x => new { x.AppUserId, x.BookId });
        builder.Entity<AppRole>().HasData(new AppRole { Id = Guid.NewGuid().ToString(), Name = "User" }, new AppRole { Id = Guid.NewGuid().ToString(), Name = "Admin" });
    }
}