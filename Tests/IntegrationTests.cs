using Xunit;
using libraryManagementSystem.Services;
using libraryManagementSystem.Models;
using System.Threading.Tasks;
using System.IO;

namespace libraryManagementSystem.Tests
{
    [Collection("Sequential")]
    public class IntegrationTests
    {
        private const string TestJsonPath = "TestData/test_books.json";
        private const string TestXmlPath = "TestData/test_members.xml";
        private const string TestBorrowPath = "TestData/test_borrow.sql";
        private const string TestLogsPath = "TestData/test_logs.txt";

        private void Cleanup() {
            if (File.Exists(TestJsonPath)) File.Delete(TestJsonPath);
            if (File.Exists(TestXmlPath)) File.Delete(TestXmlPath);
            if (File.Exists(TestBorrowPath)) File.Delete(TestBorrowPath);
            if (File.Exists(TestLogsPath)) File.Delete(TestLogsPath);
        }

        private void EnsureTestDataDir() {
            var paths = new[] { TestJsonPath, TestXmlPath, TestBorrowPath, TestLogsPath };
            foreach (var path in paths) {
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
            }
        }

        [Fact]
        public async Task LendAndReturn_EndToEnd()
        {
            EnsureTestDataDir();
            Cleanup();
            var bookService = new BookService(TestJsonPath);
            var memberService = new MemberService(TestXmlPath);
            var borrowService = new BorrowService(TestBorrowPath, TestLogsPath, bookService, memberService);
            var book = new Book { Title = "EndToEnd", Author = "A" };
            var member = new Member { Name = "EndToEndUser", Email = "e2e@test.com" };
            await bookService.AddBookAsync(book);
            await memberService.RegisterMemberAsync(member);
            var addedBook = (await bookService.GetBooksAsync()).Find(b => b.Title == "EndToEnd");
            var addedMember = (await memberService.GetMembersAsync()).Find(m => m.Name == "EndToEndUser");
            await borrowService.LendBookAsync(addedBook!.Id, addedMember!.Id);
            var record = (await borrowService.GetRecordsAsync()).Find(r => r.BookId == addedBook.Id && r.MemberId == addedMember.Id);
            Assert.NotNull(record);
            Assert.Null(record!.ReturnDate);
            await borrowService.ReturnBookAsync(record.Id);
            Assert.NotNull((await borrowService.GetRecordsAsync()).Find(r => r.Id == record.Id)!.ReturnDate);
            Cleanup();
        }
    }
}
