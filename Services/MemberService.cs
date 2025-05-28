using libraryManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace libraryManagementSystem.Services
{
    public class MemberService
    {
        private readonly List<Member> _members = new();
        public void RegisterMember(Member member)
        {
            member.Id = _members.Count > 0 ? _members.Max(m => m.Id) + 1 : 1;
            _members.Add(member);
        }
        public List<Member> GetMembers() => _members.ToList();
    }
}
