using libraryManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;

namespace libraryManagementSystem.Services
{
    /// <summary>
    /// Service for managing books with file-based storage. Uses async/await and DI for file path.
    /// </summary>
    public class BookService
    {
        private readonly string _jsonPath;
        public BookService(string jsonPath)
        {
            _jsonPath = jsonPath;
        }

        /// <summary>
        /// Loads all books from the JSON file asynchronously.
        /// </summary>
        public async Task<List<Book>> GetBooksAsync()
        {
            if (!File.Exists(_jsonPath)) return new List<Book>();
            var json = await File.ReadAllTextAsync(_jsonPath);
            return string.IsNullOrWhiteSpace(json) ? new List<Book>() :
                JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
        }

        /// <summary>
        /// Adds a new book and saves the list asynchronously.
        /// </summary>
        public async Task AddBookAsync(Book book)
        {
            var books = await GetBooksAsync();
            book.Id = books.Count > 0 ? books.Max(b => b.Id) + 1 : 1;
            book.IsAvailable = true;
            books.Add(book);
            await SaveBooksAsync(books);
        }

        /// <summary>
        /// Searches for books by title or author asynchronously.
        /// </summary>
        public async Task<List<Book>> SearchBooksAsync(string search)
        {
            var books = await GetBooksAsync();
            return books.Where(b => b.Title.Contains(search) || b.Author.Contains(search)).ToList();
        }

        /// <summary>
        /// Sets the availability of a book and saves the list asynchronously.
        /// </summary>
        public async Task SetAvailabilityAsync(int id, bool available)
        {
            var books = await GetBooksAsync();
            var book = books.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                book.IsAvailable = available;
                await SaveBooksAsync(books);
            }
        }

        /// <summary>
        /// Saves the list of books to the JSON file asynchronously.
        /// </summary>
        private async Task SaveBooksAsync(List<Book> books)
        {
            var json = JsonSerializer.Serialize(books, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_jsonPath, json);
        }
    }
}
