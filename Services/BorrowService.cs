using libraryManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace libraryManagementSystem.Services
{
    public class BorrowService
    {
        private readonly List<BorrowRecord> _records = new();
        public void LendBook(int bookId, int memberId)
        {
            _records.Add(new BorrowRecord
            {
                Id = _records.Count > 0 ? _records.Max(r => r.Id) + 1 : 1,
                BookId = bookId,
                MemberId = memberId,
                LendDate = DateTime.Now,
                ReturnDate = null
            });
        }
        public void ReturnBook(int recordId)
        {
            var rec = _records.FirstOrDefault(r => r.Id == recordId && !r.ReturnDate.HasValue);
            if (rec != null) rec.ReturnDate = DateTime.Now;
        }
        public List<BorrowRecord> GetRecords() => _records.ToList();
    }
}
