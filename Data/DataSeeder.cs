using libraryManagementSystem.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;

namespace libraryManagementSystem.Data
{
    public static class DataSeeder
    {
        public static List<Book> LoadBooksFromJson(string path)
        {
            if (!File.Exists(path)) return new List<Book>();
            var json = File.ReadAllText(path);
            return string.IsNullOrWhiteSpace(json) ? new List<Book>() :
                JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
        }

        public static List<Member> LoadMembersFromXml(string path)
        {
            if (!File.Exists(path)) return new List<Member>();
            var xml = XDocument.Load(path);
            if (xml.Root == null) return new List<Member>();
            return xml.Root.Elements("member").Select(x => new Member
            {
                Id = (int?)x.Element("id") ?? 0,
                Name = (string?)x.Element("name") ?? string.Empty,
                Email = (string?)x.Element("email") ?? string.Empty
            }).ToList();
        }

        public static List<BorrowRecord> LoadBorrowRecordsFromSql(string path)
        {
            if (!File.Exists(path)) return new List<BorrowRecord>();
            var lines = File.ReadAllLines(path);
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
        }
    }
}
