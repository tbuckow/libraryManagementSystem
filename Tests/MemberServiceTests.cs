using Xunit;
using libraryManagementSystem.Services;
using libraryManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace libraryManagementSystem.Tests
{
    [Collection("Sequential")]
    public class MemberServiceTests
    {
        private const string TestXmlPath = "TestData/test_members.xml";

        private void Cleanup() {
            if (File.Exists(TestXmlPath)) File.Delete(TestXmlPath);
        }

        private void EnsureTestDataDir() {
            var dir = Path.GetDirectoryName(TestXmlPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
        }

        [Fact]
        public async Task RegisterMember_ShouldIncreaseCount()
        {
            EnsureTestDataDir();
            Cleanup();
            var service = new MemberService(TestXmlPath);
            var initial = (await service.GetMembersAsync()).Count;
            await service.RegisterMemberAsync(new Member { Name = "Test", Email = "a@b.com" });
            Assert.Equal(initial + 1, (await service.GetMembersAsync()).Count);
            Cleanup();
        }

        [Fact]
        public async Task GetMembers_ShouldReturnRegisteredMember()
        {
            EnsureTestDataDir();
            Cleanup();
            var service = new MemberService(TestXmlPath);
            var member = new Member { Name = "Unique", Email = "u@b.com" };
            await service.RegisterMemberAsync(member);
            Assert.Contains((await service.GetMembersAsync()), m => m.Name == "Unique");
            Cleanup();
        }
    }
}
