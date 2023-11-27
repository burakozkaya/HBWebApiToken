namespace HBWebApiToken.Entity;

public class UserFavBook
{
    public int BookId { get; set; }
    public string AppUserId { get; set; }
    //Nav Property
    public AppUser AppUser { get; set; }
    public Book Book { get; set; }
}