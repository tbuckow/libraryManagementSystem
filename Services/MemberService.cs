using libraryManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

namespace libraryManagementSystem.Services
{
    /// <summary>
    /// Service for managing members with file-based XML storage. Uses async/await and DI for file path.
    /// </summary>
    public class MemberService
    {
        private readonly string _xmlPath;
        public MemberService(string xmlPath)
        {
            _xmlPath = xmlPath;
        }

        /// <summary>
        /// Loads all members from the XML file asynchronously.
        /// </summary>
        public async Task<List<Member>> GetMembersAsync()
        {
            if (!File.Exists(_xmlPath)) return new List<Member>();
            return await Task.Run(() =>
            {
                var xml = XDocument.Load(_xmlPath);
                if (xml.Root == null) return new List<Member>();
                return xml.Root.Elements("member").Select(x => new Member
                {
                    Id = (int?)x.Element("id") ?? 0,
                    Name = (string?)x.Element("name") ?? string.Empty,
                    Email = (string?)x.Element("email") ?? string.Empty
                }).ToList();
            });
        }

        /// <summary>
        /// Registers a new member and saves the list asynchronously.
        /// </summary>
        public async Task RegisterMemberAsync(Member member)
        {
            var members = await GetMembersAsync();
            member.Id = members.Count > 0 ? members.Max(m => m.Id) + 1 : 1;
            members.Add(member);
            await SaveMembersAsync(members);
        }

        /// <summary>
        /// Saves the list of members to the XML file asynchronously.
        /// </summary>
        private async Task SaveMembersAsync(List<Member> members)
        {
            await Task.Run(() =>
            {
                var xml = new XDocument(
                    new XElement("members",
                        members.Select(m =>
                            new XElement("member",
                                new XElement("id", m.Id),
                                new XElement("name", m.Name),
                                new XElement("email", m.Email)
                            )
                        )
                    )
                );
                xml.Save(_xmlPath);
            });
        }
    }
}
