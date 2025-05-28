using libraryManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace libraryManagementSystem.Services
{
    public class BookService
    {
        private readonly List<Book> _books = new();
        public void AddBook(Book book)
        {
            book.Id = _books.Count > 0 ? _books.Max(b => b.Id) + 1 : 1;
            book.IsAvailable = true;
            _books.Add(book);
        }
        public List<Book> GetBooks() => _books.ToList();
        public List<Book> SearchBooks(string search)
        {
            return _books.Where(b => b.Title.Contains(search) || b.Author.Contains(search)).ToList();
        }
        public void SetAvailability(int id, bool available)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book != null) book.IsAvailable = available;
        }
    }
}
