using Xunit;
using libraryManagementSystem.Services;
using libraryManagementSystem.Models;
using System.Collections.Generic;

namespace libraryManagementSystem.Tests
{
    public class MemberServiceTests
    {
        [Fact]
        public void RegisterMember_ShouldIncreaseCount()
        {
            var service = new MemberService();
            var initial = service.GetMembers().Count;
            service.RegisterMember(new Member { Name = "Test", Email = "a@b.com" });
            Assert.Equal(initial + 1, service.GetMembers().Count);
        }

        [Fact]
        public void GetMembers_ShouldReturnRegisteredMember()
        {
            var service = new MemberService();
            var member = new Member { Name = "Unique", Email = "u@b.com" };
            service.RegisterMember(member);
            Assert.Contains(service.GetMembers(), m => m.Name == "Unique");
        }
    }
}
