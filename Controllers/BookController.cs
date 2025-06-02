using Microsoft.AspNetCore.Mvc;
using libraryManagementSystem.Models;
using libraryManagementSystem.Services;
using System.Threading.Tasks;

namespace libraryManagementSystem.Controllers
{
    /// <summary>
    /// Controller for book management. Uses BookService via dependency injection.
    /// </summary>
    public class BookController : Controller
    {
        private readonly BookService _bookService;

        // BookService is injected via constructor
        public BookController(BookService bookService)
        {
            _bookService = bookService;
        }

        /// <summary>
        /// Displays all books, optionally filtered by search string.
        /// </summary>
        public async Task<IActionResult> Index(string? search)
        {
            var books = await _bookService.GetBooksAsync();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lower = search.ToLower();
                books = books.Where(b => b.Title.ToLower().Contains(lower) || b.Author.ToLower().Contains(lower)).ToList();
            }
            ViewBag.Search = search;
            return View(books);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Book book)
        {
            await _bookService.AddBookAsync(book);
            return RedirectToAction("Index");
        }
    }
}