using Microsoft.AspNetCore.Identity;

namespace HBWebApiToken.Entity;

public class AppUser : IdentityUser<string>
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public DateTime BirthDate { get; set; }
    public List<UserFavBook> FavBooks { get; set; }
}