using Microsoft.AspNetCore.Mvc;
using libraryManagementSystem.Models;
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
            if (!System.IO.File.Exists(_xmlPath))
                return new List<Member>();
            var xml = XDocument.Load(_xmlPath);
            return xml.Root == null ? new List<Member>() :
                xml.Root.Elements("member").Select(x => new Member
                {
                    Id = (int?)x.Element("id") ?? 0,
                    Name = (string?)x.Element("name") ?? string.Empty,
                    Email = (string?)x.Element("email") ?? string.Empty
                }).ToList();
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