using Xunit;
using libraryManagementSystem.Models;
using System.IO;
using System.Linq;

namespace libraryManagementSystem.Tests
{
    public class DataSeederTests
    {
        [Fact]
        public void LoadBooks_FromJson_ShouldReturnBooks()
        {
            var path = "Data/books.json";
            var json = File.ReadAllText(path);
            var books = System.Text.Json.JsonSerializer.Deserialize<Book[]>(json);
            Assert.NotNull(books);
            Assert.All(books, b => Assert.False(string.IsNullOrWhiteSpace(b.Title)));
        }

        [Fact]
        public void LoadMembers_FromXml_ShouldReturnMembers()
        {
            var path = "Data/members.xml";
            var xml = System.Xml.Linq.XDocument.Load(path);
            Assert.NotNull(xml.Root); // Ensure Root is not null
            var members = xml.Root != null ? xml.Root.Elements("member").Select(x => new Member
            {
                Id = (int?)x.Element("id") ?? 0,
                Name = (string?)x.Element("name") ?? string.Empty,
                Email = (string?)x.Element("email") ?? string.Empty
            }).ToList() : new List<Member>();
            Assert.NotEmpty(members);
        }

        [Fact]
        public void LoadBorrowRecords_FromSql_ShouldReturnRecords()
        {
            var path = "Data/borrow.sql";
            var lines = File.ReadAllLines(path);
            var records = lines.Select(line => line.Split(';')).ToList();
            Assert.All(records, r => Assert.True(r.Length >= 4));
        }
    }
}
