using HBWebApiToken.Context;
using HBWebApiToken.Entity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HBWebApiToken.Controllers
{
    [Authorize(policy:)]
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public BookController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet("userFavBooks")]
        public async Task<IActionResult> GetUserFavBooks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var books = await _appDbContext.FavBooks
                .Include(x => x.Book)
                .Where(x => x.AppUserId == userId)
                .Select(x => x.Book)
                .ToListAsync();

            return Ok(books);
        }

        [HttpPost("AddFavBook/{id}")]
        public async Task<IActionResult> AddFavBook(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var exists = await _appDbContext.Books.AnyAsync(x => x.Id == id) && await _appDbContext.Users.AnyAsync(x => x.Id == userId);

            if (exists)
            {
                var userFavBook = new UserFavBook
                {
                    AppUserId = userId,
                    BookId = id
                };

                _appDbContext.FavBooks.Add(userFavBook);
                await _appDbContext.SaveChangesAsync();

                return Ok();
            }

            return NotFound();
        }


        [HttpGet]
        public async Task<IActionResult> GetBooks()
        {
            var books = await _appDbContext.Books.ToListAsync();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _appDbContext.Books.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(book);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddBook(BookDto bookDto)
        {
            Book book = new Book()
            {
                BookName = bookDto.BookName,
                CategoryName = bookDto.CategoryName,
                AuthorName = bookDto.AuthorName,
                Page = bookDto.Page,
                Color = bookDto.Color
            };
            await _appDbContext.Books.AddAsync(book);
            await _appDbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created);
        }
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var temp = _appDbContext.Books.FirstOrDefault(x => x.Id == id);
            if (temp != null)
            {
                _appDbContext.Books.Remove(temp);
                await _appDbContext.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpGet("listBook")]
        public async Task<IActionResult> ListBooks([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            if (pageNumber <= 0)
                pageNumber = 1;
            if (pageSize <= 0)
                pageSize = 50;

            var books = await _appDbContext.Books.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return Ok(books);
        }

        [HttpGet("filterBooks")]
        public async Task<IActionResult> FilterBooks(Filter? filter, [FromQuery] int pageNumber,
            [FromQuery] int pageSize)
        {
            var temp = _appDbContext.Books.AsQueryable();
            if (filter.MaxPage <= 0)
                temp = temp.Where(x => x.Page < filter.MaxPage);
            if (filter.MinPage >= 0)
                temp = temp.Where(x => x.Page > filter.MinPage);
            if (!string.IsNullOrWhiteSpace(filter.CategoryName))
                temp = temp.Where(x => x.CategoryName == filter.CategoryName);
            if (!string.IsNullOrWhiteSpace(filter.Color))
                temp = temp.Where(x => x.AuthorName == filter.Color);
            if (!string.IsNullOrWhiteSpace(filter.AuthorName))
                temp = temp.Where(x => x.AuthorName == filter.AuthorName);
            return Ok(await temp.ToListAsync());
        }
    }

    public class BookDto
    {
        public string BookName { get; set; }
        public string CategoryName { get; set; }
        public string AuthorName { get; set; }
        public int Page { get; set; }
        public string Color { get; set; }
    }

    public class Filter
    {
        public int? MaxPage { get; set; }
        public int? MinPage { get; set; }
        public string? CategoryName { get; set; }
        public string? Color { get; set; }
        public string? AuthorName { get; set; }
    }
}
