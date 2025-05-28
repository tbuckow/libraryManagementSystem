using Xunit;
using libraryManagementSystem.Services;
using libraryManagementSystem.Models;
using System.Collections.Generic;

namespace libraryManagementSystem.Tests
{
    public class BookServiceTests
    {
        [Fact]
        public void AddBook_ShouldIncreaseBookCount()
        {
            var service = new BookService();
            var initialCount = service.GetBooks().Count;
            service.AddBook(new Book { Title = "Test", Author = "A" });
            Assert.Equal(initialCount + 1, service.GetBooks().Count);
        }

        [Fact]
        public void SearchBook_ShouldReturnCorrectBook()
        {
            var service = new BookService();
            var book = new Book { Title = "UniqueTitle", Author = "A" };
            service.AddBook(book);
            var result = service.SearchBooks("UniqueTitle");
            Assert.Contains(result, b => b.Title == "UniqueTitle");
        }

        [Fact]
        public void BookAvailability_ShouldBeUpdated()
        {
            var service = new BookService();
            var book = new Book { Title = "AvailTest", Author = "A" };
            service.AddBook(book);
            var added = service.GetBooks().Find(b => b.Title == "AvailTest");
            Assert.NotNull(added); // Ensure not null before dereference
            Assert.True(added!.IsAvailable);
            service.SetAvailability(added.Id, false);
            var updated = service.GetBooks().Find(b => b.Id == added.Id);
            Assert.NotNull(updated); // Ensure not null before dereference
            Assert.False(updated!.IsAvailable);
        }
    }
}
