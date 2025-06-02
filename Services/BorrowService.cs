using libraryManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Text.Json;

namespace libraryManagementSystem.Services
{
    /// <summary>
    /// Service for managing borrow records with file-based storage. Uses async/await and DI for file paths.
    /// </summary>
    public class BorrowService
    {
        private readonly string _borrowPath;
        private readonly string _logsPath;
        private readonly BookService _bookService;
        private readonly MemberService _memberService;

        public BorrowService(string borrowPath, string logsPath, BookService bookService, MemberService memberService)
        {
            _borrowPath = borrowPath;
            _logsPath = logsPath;
            _bookService = bookService;
            _memberService = memberService;
        }

        /// <summary>
        /// Loads all borrow records from the flat file asynchronously.
        /// </summary>
        public async Task<List<BorrowRecord>> GetRecordsAsync()
        {
            if (!File.Exists(_borrowPath)) return new List<BorrowRecord>();
            return await Task.Run(() =>
            {
                var lines = File.ReadAllLines(_borrowPath);
                return lines.Select(line =>
                {
                    var parts = line.Split(';');
                    return new BorrowRecord
                    {
                        Id = int.Parse(parts[0]),
                        BookId = int.Parse(parts[1]),
                        MemberId = int.Parse(parts[2]),
                        LendDate = DateTime.Parse(parts[3]),
                        ReturnDate = parts.Length > 4 && !string.IsNullOrWhiteSpace(parts[4]) ? DateTime.Parse(parts[4]) : (DateTime?)null
                    };
                }).ToList();
            });
        }

        /// <summary>
        /// Lends a book to a member and logs the action asynchronously.
        /// </summary>
        public async Task<bool> LendBookAsync(int bookId, int memberId)
        {
            var records = await GetRecordsAsync();
            var newRecord = new BorrowRecord
            {
                Id = records.Count > 0 ? records.Max(r => r.Id) + 1 : 1,
                BookId = bookId,
                MemberId = memberId,
                LendDate = DateTime.Now,
                ReturnDate = null
            };
            records.Add(newRecord);
            await SaveRecordsAsync(records);
            await _bookService.SetAvailabilityAsync(bookId, false);
            var books = await _bookService.GetBooksAsync();
            var members = await _memberService.GetMembersAsync();
            var book = books.FirstOrDefault(b => b.Id == bookId);
            var member = members.FirstOrDefault(m => m.Id == memberId);
            if (book != null && member != null)
            {
                await LogActionAsync($"LEND: Book '{book.Title}' (ID: {book.Id}) lent to Member '{member.Name}' (ID: {member.Id})");
            }
            return true;
        }

        /// <summary>
        /// Returns a book and logs the action asynchronously.
        /// </summary>
        public async Task<bool> ReturnBookAsync(int recordId)
        {
            var records = await GetRecordsAsync();
            var record = records.FirstOrDefault(r => r.Id == recordId && !r.ReturnDate.HasValue);
            if (record != null)
            {
                record.ReturnDate = DateTime.Now;
                await SaveRecordsAsync(records);
                await _bookService.SetAvailabilityAsync(record.BookId, true);
                var books = await _bookService.GetBooksAsync();
                var members = await _memberService.GetMembersAsync();
                var book = books.FirstOrDefault(b => b.Id == record.BookId);
                var member = members.FirstOrDefault(m => m.Id == record.MemberId);
                if (book != null && member != null)
                {
                    await LogActionAsync($"RETURN: Book '{book.Title}' (ID: {book.Id}) returned by Member '{member.Name}' (ID: {member.Id})");
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Saves the list of borrow records to the flat file asynchronously.
        /// </summary>
        private async Task SaveRecordsAsync(List<BorrowRecord> records)
        {
            await Task.Run(() =>
            {
                var lines = records.Select(r => $"{r.Id};{r.BookId};{r.MemberId};{r.LendDate:yyyy-MM-dd};{(r.ReturnDate.HasValue ? r.ReturnDate.Value.ToString("yyyy-MM-dd") : "")}");
                File.WriteAllLines(_borrowPath, lines);
            });
        }

        /// <summary>
        /// Logs an action to the logs file asynchronously.
        /// </summary>
        private async Task LogActionAsync(string message)
        {
            var logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            await File.AppendAllTextAsync(_logsPath, logLine + Environment.NewLine);
        }
    }
}
