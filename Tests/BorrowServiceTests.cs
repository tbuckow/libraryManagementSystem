using Xunit;
using libraryManagementSystem.Services;
using libraryManagementSystem.Models;

namespace libraryManagementSystem.Tests
{
    public class BorrowServiceTests
    {
        [Fact]
        public void LendBook_ShouldAddRecord()
        {
            var service = new BorrowService();
            service.LendBook(1, 1);
            Assert.Single(service.GetRecords());
        }

        [Fact]
        public void ReturnBook_ShouldSetReturnDate()
        {
            var service = new BorrowService();
            service.LendBook(1, 1);
            var record = service.GetRecords()[0];
            Assert.Null(record.ReturnDate);
            service.ReturnBook(record.Id);
            Assert.NotNull(service.GetRecords()[0].ReturnDate);
        }
    }
}
