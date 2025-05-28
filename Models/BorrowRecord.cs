namespace libraryManagementSystem.Models
{
    public class BorrowRecord
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int MemberId { get; set; }
        public DateTime LendDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}