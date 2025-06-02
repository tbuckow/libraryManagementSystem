using Xunit;
using libraryManagementSystem.Services;
using libraryManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace libraryManagementSystem.Tests
{
    [Collection("Sequential")]
    public class BookServiceTests
    {
        private const string TestJsonPath = "TestData/test_books.json";

        private void Cleanup() {
            if (File.Exists(TestJsonPath)) File.Delete(TestJsonPath);
        }

        private void EnsureTestDataDir() {
            var dir = Path.GetDirectoryName(TestJsonPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
        }

        [Fact]
        public async Task AddBook_ShouldIncreaseBookCount()
        {
            EnsureTestDataDir();
            Cleanup();
            var service = new BookService(TestJsonPath);
            var initialCount = (await service.GetBooksAsync()).Count;
            await service.AddBookAsync(new Book { Title = "Test", Author = "A" });
            Assert.Equal(initialCount + 1, (await service.GetBooksAsync()).Count);
            Cleanup();
        }

        [Fact]
        public async Task SearchBook_ShouldReturnCorrectBook()
        {
            EnsureTestDataDir();
            Cleanup();
            var service = new BookService(TestJsonPath);
            var book = new Book { Title = "UniqueTitle", Author = "A" };
            await service.AddBookAsync(book);
            var result = await service.SearchBooksAsync("UniqueTitle");
            Assert.Contains(result, b => b.Title == "UniqueTitle");
            Cleanup();
        }

        [Fact]
        public async Task BookAvailability_ShouldBeUpdated()
        {
            EnsureTestDataDir();
            Cleanup();
            var service = new BookService(TestJsonPath);
            var book = new Book { Title = "AvailTest", Author = "A" };
            await service.AddBookAsync(book);
            var added = (await service.GetBooksAsync()).Find(b => b.Title == "AvailTest");
            Assert.NotNull(added); // Ensure not null before dereference
            Assert.True(added!.IsAvailable);
            await service.SetAvailabilityAsync(added.Id, false);
            var updated = (await service.GetBooksAsync()).Find(b => b.Id == added.Id);
            Assert.NotNull(updated); // Ensure not null before dereference
            Assert.False(updated!.IsAvailable);
            Cleanup();
        }
    }
}
