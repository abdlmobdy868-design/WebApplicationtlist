using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebApplicationtodolist.Data;
using WebApplicationtodolist.Models;

[Authorize]
public class AssignmentsController : Controller
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public AssignmentsController(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    // List for students and instructors
    public IActionResult Index()
    {
        var list = _db.Assignments.OrderByDescending(a => a.CreatedAt).ToList();
        return View(list);
    }

    public IActionResult Details(int id)
    {
        var assignment = _db.Assignments.Find(id);
        if (assignment == null) return NotFound();
        return View(assignment);
    }

    // Create/Edit/Delete actions for instructors only - simple implementation
    public IActionResult Create()
    {
        return View(new Assignment());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Assignment a, IFormFile file)
    {
        if (file != null)
        {
            var uploads = Path.Combine(_env.WebRootPath, "uploads", "assignments");
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            using (var stream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                await file.CopyToAsync(stream);
            a.FilePath = fileName;
        }

        _db.Assignments.Add(a);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
