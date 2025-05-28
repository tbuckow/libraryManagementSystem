using Microsoft.EntityFrameworkCore;
using libraryManagementSystem.Models;

namespace libraryManagementSystem.Models // <- Korrigiere Namespace zu .Data
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }
        public DbSet<Book> Books { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }
    }
}