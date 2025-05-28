using Microsoft.AspNetCore.Mvc;
using libraryManagementSystem.Models;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace libraryManagementSystem.Controllers
{
    public class BookController : Controller
    {
        private readonly string _jsonPath = "Data/books.json";

        private List<Book> LoadBooks()
        {
            if (!System.IO.File.Exists(_jsonPath))
                return new List<Book>();
            var json = System.IO.File.ReadAllText(_jsonPath);
            if (string.IsNullOrWhiteSpace(json))
                return new List<Book>();
            return JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
        }

        private void SaveBooks(List<Book> books)
        {
            var json = JsonSerializer.Serialize(books, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(_jsonPath, json);
        }

        public IActionResult Index(string? search)
        {
            var books = LoadBooks();
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
        public IActionResult Add(Book book)
        {
            var books = LoadBooks();
            book.Id = books.Count > 0 ? books.Max(b => b.Id) + 1 : 1;
            book.IsAvailable = true;
            books.Add(book);
            SaveBooks(books);
            return RedirectToAction("Index");
        }
    }
}