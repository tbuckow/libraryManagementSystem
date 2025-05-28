using Microsoft.AspNetCore.Mvc;
using libraryManagementSystem.Models;
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
            if (!System.IO.File.Exists(_booksPath))
                return new List<Book>();
            var json = System.IO.File.ReadAllText(_booksPath);
            if (string.IsNullOrWhiteSpace(json))
                return new List<Book>();
            return JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
        }

        private void SaveBooks(List<Book> books)
        {
            var json = JsonSerializer.Serialize(books, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(_booksPath, json);
        }

        private List<Member> LoadMembers()
        {
            if (!System.IO.File.Exists(_membersPath))
                return new List<Member>();
            var xml = XDocument.Load(_membersPath);
            return xml.Root == null ? new List<Member>() :
                xml.Root.Elements("member").Select(x => new Member
                {
                    Id = (int?)x.Element("id") ?? 0,
                    Name = (string?)x.Element("name") ?? string.Empty,
                    Email = (string?)x.Element("email") ?? string.Empty
                }).ToList();
        }

        private List<BorrowRecord> LoadBorrowRecords()
        {
            if (!System.IO.File.Exists(_borrowPath))
                return new List<BorrowRecord>();
            var lines = System.IO.File.ReadAllLines(_borrowPath);
            return lines.Select(line =>
            {
                var parts = line.Split(';');
                return new BorrowRecord
                {
                    Id = int.Parse(parts[0]),
                    BookId = int.Parse(parts[1]),
                    MemberId = int.Parse(parts[2]),
                    LendDate = DateTime.Parse(parts[3]),
                    ReturnDate = string.IsNullOrWhiteSpace(parts[4]) ? null : DateTime.Parse(parts[4])
                };
            }).ToList();
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