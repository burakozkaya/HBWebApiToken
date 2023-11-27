using HBWebApiToken.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HBWebApiToken.Context;

public class AppDbContext:IdentityDbContext<AppUser,AppRole,string>
{
    public DbSet<Book> Books { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
    {
        
    }
}