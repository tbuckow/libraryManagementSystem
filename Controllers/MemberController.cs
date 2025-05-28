using Microsoft.AspNetCore.Mvc;
using libraryManagementSystem.Models;
using libraryManagementSystem.Data; // Add this using
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace libraryManagementSystem.Controllers
{
    public class MemberController : Controller
    {
        private readonly string _xmlPath = "Data/members.xml";

        private List<Member> LoadMembers()
        {
            return DataSeeder.LoadMembersFromXml(_xmlPath);
        }

        private void SaveMembers(List<Member> members)
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
        }

        public IActionResult Index()
        {
            var members = LoadMembers();
            return View(members);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(Member member)
        {
            var members = LoadMembers();
            member.Id = members.Count > 0 ? members.Max(m => m.Id) + 1 : 1;
            members.Add(member);
            SaveMembers(members);
            return RedirectToAction("Index");
        }
    }
}