using System.Security.Claims;
using HBWebApiToken.Context;
using HBWebApiToken.Entity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HBWebApiToken.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        public IActionResult GetUserFavBooks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var books = _appDbContext.Books.Where(x => x.AppUsers.Any(x=>x.Id == userId)).ToList();
            return Ok(books);
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
        public async Task<IActionResult> AddBook(BookDto bookDto)
        {
            Book book = new Book()
            {
                BookName = bookDto.BookName,
                CategoryName = bookDto.CategoryName,
                Page = bookDto.Page,
                Color = bookDto.Color
            };
            await _appDbContext.Books.AddAsync(book);
            await _appDbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpGet("listBook")]
        public async Task<IActionResult> ListBooks(int pagenumber, int pagesize)
        {
            var books = await _appDbContext.Books.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToListAsync();
            return Ok(books);
        }
    }

    public class BookDto
    {
        public string BookName { get; set; }
        public string CategoryName { get; set; }
        public int Page { get; set; }
        public string Color { get; set; }
    }
}
