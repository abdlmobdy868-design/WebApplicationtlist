using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplicationtodolist.Models;
using WebApplicationtodolist.Data;
using Microsoft.EntityFrameworkCore;


using Microsoft.AspNetCore.Authorization;

namespace WebApplicationtodolist.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            // Build a lightweight dashboard model with simple counts
            var now = DateTime.Now;
            var total = _db.TodoItems.Count();
            var overdue = _db.TodoItems.Count(t => t.Deadline < now);
            var dueSoon = _db.TodoItems.Count(t => t.Deadline >= now && t.Deadline <= now.AddDays(7));
            var withFiles = _db.TodoItems.Count(t => !string.IsNullOrEmpty(t.FilePath));

            var model = new DashboardViewModel
            {
                TotalTasks = total,
                OverdueTasks = overdue,
                DueSoonTasks = dueSoon,
                WithFiles = withFiles
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
