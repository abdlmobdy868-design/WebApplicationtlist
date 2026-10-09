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

    public AdminController(AppDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
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
    public async Task<IActionResult> MakeAdmin(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null && !await _userManager.IsInRoleAsync(user, "Admin"))
        {
            await _userManager.AddToRoleAsync(user, "Admin");
        }
        return RedirectToAction(nameof(Users));
    }

    public async Task<IActionResult> Courses()
    {
        var courses = await _context.CourseMaterials.ToListAsync();
        return View(courses);
    }
}
