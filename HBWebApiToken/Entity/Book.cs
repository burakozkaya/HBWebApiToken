﻿namespace HBWebApiToken.Entity;

public class Book
{
    public int Id { get; set; }
    public string BookName { get; set; }
    public string AuthorName { get; set; }
    public string CategoryName { get; set; }
    public int Page { get; set; }
    public string Color { get; set; }
    public List<UserFavBook> FavBooks { get; set; }
}