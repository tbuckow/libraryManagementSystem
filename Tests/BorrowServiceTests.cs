using Xunit;
using libraryManagementSystem.Services;
using libraryManagementSystem.Models;
using System.Threading.Tasks;
using System.IO;

namespace libraryManagementSystem.Tests
{
    [Collection("Sequential")]
    public class BorrowServiceTests
    {
        private const string TestBorrowPath = "TestData/test_borrow.sql";
        private const string TestBooksPath = "TestData/test_books.json";
        private const string TestMembersPath = "TestData/test_members.xml";
        private const string TestLogsPath = "TestData/test_logs.txt";

        private void Cleanup() {
            if (File.Exists(TestBorrowPath)) File.Delete(TestBorrowPath);
            if (File.Exists(TestBooksPath)) File.Delete(TestBooksPath);
            if (File.Exists(TestMembersPath)) File.Delete(TestMembersPath);
            if (File.Exists(TestLogsPath)) File.Delete(TestLogsPath);
        }

        private void EnsureTestDataDir() {
            var paths = new[] { TestBorrowPath, TestBooksPath, TestMembersPath, TestLogsPath };
            foreach (var path in paths) {
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
            }
        }

        [Fact]
        public async Task LendBook_ShouldAddRecord()
        {
            EnsureTestDataDir();
            Cleanup();
            var bookService = new BookService(TestBooksPath);
            var memberService = new MemberService(TestMembersPath);
            var borrowService = new BorrowService(TestBorrowPath, TestLogsPath, bookService, memberService);
            await bookService.AddBookAsync(new Book { Title = "Test", Author = "A" });
            await memberService.RegisterMemberAsync(new Member { Name = "Test", Email = "a@b.com" });
            var book = (await bookService.GetBooksAsync())[0];
            var member = (await memberService.GetMembersAsync())[0];
            await borrowService.LendBookAsync(book.Id, member.Id);
            Assert.Single(await borrowService.GetRecordsAsync());
            Cleanup();
        }

        [Fact]
        public async Task ReturnBook_ShouldSetReturnDate()
        {
            EnsureTestDataDir();
            Cleanup();
            var bookService = new BookService(TestBooksPath);
            var memberService = new MemberService(TestMembersPath);
            var borrowService = new BorrowService(TestBorrowPath, TestLogsPath, bookService, memberService);
            await bookService.AddBookAsync(new Book { Title = "Test", Author = "A" });
            await memberService.RegisterMemberAsync(new Member { Name = "Test", Email = "a@b.com" });
            var book = (await bookService.GetBooksAsync())[0];
            var member = (await memberService.GetMembersAsync())[0];
            await borrowService.LendBookAsync(book.Id, member.Id);
            var record = (await borrowService.GetRecordsAsync())[0];
            Assert.Null(record.ReturnDate);
            await borrowService.ReturnBookAsync(record.Id);
            Assert.NotNull((await borrowService.GetRecordsAsync())[0].ReturnDate);
            Cleanup();
        }
    }
}
