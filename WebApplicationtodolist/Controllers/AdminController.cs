using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationtodolist.Data;
using WebApplicationtodolist.Models;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(AppDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var model = new AdminDashboardViewModel
        {
            TotalUsers = await _userManager.Users.CountAsync(),
            TotalCourses = await _context.CourseMaterials.CountAsync(),
            TotalTasks = await _context.TodoItems.CountAsync()
        };
        return View(model);
    }

    public async Task<IActionResult> Users()
    {
        var users = await _userManager.Users.ToListAsync();
        var list = new List<AdminUserViewModel>();
        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            list.Add(new AdminUserViewModel { Id = u.Id, UserName = u.UserName, Email = u.Email, Roles = roles });
        }
        return View(list);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MakeAdmin(string id)
    {
        if (string.IsNullOrEmpty(id)) return RedirectToAction(nameof(Users));
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            if (!await _userManager.IsInRoleAsync(user, "Admin"))
                await _userManager.AddToRoleAsync(user, "Admin");
            // remove Student role if exists
            if (await _userManager.IsInRoleAsync(user, "Student"))
                await _userManager.RemoveFromRoleAsync(user, "Student");
        }
        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MakeStudent(string id)
    {
        if (string.IsNullOrEmpty(id)) return RedirectToAction(nameof(Users));
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            if (!await _roleManager.RoleExistsAsync("Student"))
                await _roleManager.CreateAsync(new IdentityRole("Student"));
            if (!await _userManager.IsInRoleAsync(user, "Student"))
                await _userManager.AddToRoleAsync(user, "Student");
            // remove Admin role if exists
            if (await _userManager.IsInRoleAsync(user, "Admin"))
                await _userManager.RemoveFromRoleAsync(user, "Admin");
        }
        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string id)
    {
        if (string.IsNullOrEmpty(id)) return RedirectToAction(nameof(Users));
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            // prevent deleting self
            var currentId = _userManager.GetUserId(User);
            if (user.Id == currentId)
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Users));
            }
            await _userManager.DeleteAsync(user);
        }
        return RedirectToAction(nameof(Users));
    }

    public async Task<IActionResult> Courses()
    {
        var courses = await _context.CourseMaterials.ToListAsync();
        return View(courses);
    }
}
