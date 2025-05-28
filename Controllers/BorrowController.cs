using Microsoft.AspNetCore.Mvc;
using libraryManagementSystem.Models;
using libraryManagementSystem.Data; // Add this using
using System.Text.Json;
using System.Xml.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace libraryManagementSystem.Controllers
{
    public class BorrowController : Controller
    {
        private readonly string _booksPath = "Data/books.json";
        private readonly string _membersPath = "Data/members.xml";
        private readonly string _borrowPath = "Data/borrow.sql";
        private readonly string _logsPath = "Data/logs.txt";

        private List<Book> LoadBooks()
        {
            return DataSeeder.LoadBooksFromJson(_booksPath);
        }

        private void SaveBooks(List<Book> books)
        {
            var json = JsonSerializer.Serialize(books, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(_booksPath, json);
        }

        private List<Member> LoadMembers()
        {
            return DataSeeder.LoadMembersFromXml(_membersPath);
        }

        private List<BorrowRecord> LoadBorrowRecords()
        {
            return DataSeeder.LoadBorrowRecordsFromSql(_borrowPath);
        }

        private void SaveBorrowRecords(List<BorrowRecord> records)
        {
            var lines = records.Select(r => $"{r.Id};{r.BookId};{r.MemberId};{r.LendDate:yyyy-MM-dd};{(r.ReturnDate.HasValue ? r.ReturnDate.Value.ToString("yyyy-MM-dd") : "")}");
            System.IO.File.WriteAllLines(_borrowPath, lines);
        }

        private void LogAction(string message)
        {
            var logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            System.IO.File.AppendAllText(_logsPath, logLine + System.Environment.NewLine);
        }

        public IActionResult Index()
        {
            var records = LoadBorrowRecords();
            var books = LoadBooks();
            var members = LoadMembers();
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
        public IActionResult Lend()
        {
            ViewBag.Books = LoadBooks().Where(b => b.IsAvailable).ToList();
            ViewBag.Members = LoadMembers();
            return View();
        }

        [HttpPost]
        public IActionResult Lend(int bookId, int memberId)
        {
            var books = LoadBooks();
            var book = books.FirstOrDefault(b => b.Id == bookId && b.IsAvailable);
            if (book == null)
            {
                ModelState.AddModelError("BookId", "Book not available.");
                ViewBag.Books = books.Where(b => b.IsAvailable).ToList();
                ViewBag.Members = LoadMembers();
                return View();
            }
            var members = LoadMembers();
            var member = members.FirstOrDefault(m => m.Id == memberId);
            var records = LoadBorrowRecords();
            var newRecord = new BorrowRecord
            {
                Id = records.Count > 0 ? records.Max(r => r.Id) + 1 : 1,
                BookId = bookId,
                MemberId = memberId,
                LendDate = DateTime.Now,
                ReturnDate = null
            };
            records.Add(newRecord);
            SaveBorrowRecords(records);
            book.IsAvailable = false;
            SaveBooks(books);
            if (book != null && member != null)
            {
                LogAction($"LEND: Book '{book.Title}' (ID: {book.Id}) lent to Member '{member.Name}' (ID: {member.Id})");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Return(int id)
        {
            var records = LoadBorrowRecords();
            var record = records.FirstOrDefault(r => r.Id == id && !r.ReturnDate.HasValue);
            if (record != null)
            {
                record.ReturnDate = DateTime.Now;
                SaveBorrowRecords(records);
                var books = LoadBooks();
                var book = books.FirstOrDefault(b => b.Id == record.BookId);
                var members = LoadMembers();
                var member = members.FirstOrDefault(m => m.Id == record.MemberId);
                if (book != null)
                {
                    book.IsAvailable = true;
                    SaveBooks(books);
                }
                if (book != null && member != null)
                {
                    LogAction($"RETURN: Book '{book.Title}' (ID: {book.Id}) returned by Member '{member.Name}' (ID: {member.Id})");
                }
            }
            return RedirectToAction("Index");
        }
    }
}