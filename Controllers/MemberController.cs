using Microsoft.AspNetCore.Mvc;
using libraryManagementSystem.Models;
using libraryManagementSystem.Services;
using System.Threading.Tasks;

namespace libraryManagementSystem.Controllers
{
    /// <summary>
    /// Controller for member management. Uses MemberService via dependency injection.
    /// </summary>
    public class MemberController : Controller
    {
        private readonly MemberService _memberService;

        // MemberService is injected via constructor
        public MemberController(MemberService memberService)
        {
            _memberService = memberService;
        }

        /// <summary>
        /// Displays all members.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var members = await _memberService.GetMembersAsync();
            return View(members);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Member member)
        {
            await _memberService.RegisterMemberAsync(member);
            return RedirectToAction("Index");
        }
    }
}