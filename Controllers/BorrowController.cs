using Microsoft.AspNetCore.Mvc;
using libraryManagementSystem.Models;
using libraryManagementSystem.Services;
using System.Threading.Tasks;
using System.Linq;

namespace libraryManagementSystem.Controllers
{
    /// <summary>
    /// Controller for borrowing and returning books. Uses BorrowService, BookService, and MemberService via dependency injection.
    /// </summary>
    public class BorrowController : Controller
    {
        private readonly BorrowService _borrowService;
        private readonly BookService _bookService;
        private readonly MemberService _memberService;

        // Services are injected via constructor
        public BorrowController(BorrowService borrowService, BookService bookService, MemberService memberService)
        {
            _borrowService = borrowService;
            _bookService = bookService;
            _memberService = memberService;
        }

        /// <summary>
        /// Displays borrowing history.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var records = await _borrowService.GetRecordsAsync();
            var books = await _bookService.GetBooksAsync();
            var members = await _memberService.GetMembersAsync();
            var view = records.Select(r => new
            {
                Record = r,
                Book = books.FirstOrDefault(b => b.Id == r.BookId),
                Member = members.FirstOrDefault(m => m.Id == r.MemberId)
            }).ToList();
            ViewBag.Books = books;
            ViewBag.Members = members;
            return View(view);
        }

        [HttpGet]
        public async Task<IActionResult> Lend()
        {
            var books = (await _bookService.GetBooksAsync()).Where(b => b.IsAvailable).ToList();
            var members = await _memberService.GetMembersAsync();
            ViewBag.Books = books;
            ViewBag.Members = members;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Lend(int bookId, int memberId)
        {
            var books = await _bookService.GetBooksAsync();
            var book = books.FirstOrDefault(b => b.Id == bookId && b.IsAvailable);
            if (book == null)
            {
                ModelState.AddModelError("BookId", "Book not available.");
                ViewBag.Books = books.Where(b => b.IsAvailable).ToList();
                ViewBag.Members = await _memberService.GetMembersAsync();
                return View();
            }
            await _borrowService.LendBookAsync(bookId, memberId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Return(int id)
        {
            await _borrowService.ReturnBookAsync(id);
            return RedirectToAction("Index");
        }
    }
}