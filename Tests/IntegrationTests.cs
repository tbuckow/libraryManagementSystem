using Xunit;
using libraryManagementSystem.Services;
using libraryManagementSystem.Models;

namespace libraryManagementSystem.Tests
{
    public class IntegrationTests
    {
        [Fact]
        public void LendAndReturn_EndToEnd()
        {
            var bookService = new BookService();
            var memberService = new MemberService();
            var borrowService = new BorrowService();
            var book = new Book { Title = "EndToEnd", Author = "A" };
            var member = new Member { Name = "EndToEndUser", Email = "e2e@test.com" };
            bookService.AddBook(book);
            memberService.RegisterMember(member);
            var addedBook = bookService.GetBooks().Find(b => b.Title == "EndToEnd");
            var addedMember = memberService.GetMembers().Find(m => m.Name == "EndToEndUser");
            borrowService.LendBook(addedBook.Id, addedMember.Id);
            var record = borrowService.GetRecords().Find(r => r.BookId == addedBook.Id && r.MemberId == addedMember.Id);
            Assert.NotNull(record);
            Assert.Null(record.ReturnDate);
            borrowService.ReturnBook(record.Id);
            Assert.NotNull(borrowService.GetRecords().Find(r => r.Id == record.Id).ReturnDate);
        }
    }
}
