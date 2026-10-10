using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebApplicationtodolist.Data;
using WebApplicationtodolist.Models;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class MaterialsController : Controller
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;

    public MaterialsController(AppDbContext db, IWebHostEnvironment env, IConfiguration config)
    {
        _db = db;
        _env = env;
        _config = config;
    }

    // List materials with optional search
    [Authorize]
    public async Task<IActionResult> Index(string search)
    {
        try
        {
            var q = _db.CourseMaterials.AsQueryable();
            if (!string.IsNullOrEmpty(search))
                q = q.Where(m => m.Title.Contains(search) || (m.Description ?? "").Contains(search));
            var list = await q.OrderByDescending(m => m.UploadDate).ToListAsync();
            return View(list ?? new List<CourseMaterial>());
        }
        catch
        {
            // on error return empty list to avoid exception page
            return View(new List<CourseMaterial>());
        }
    }

    // Download material
    public IActionResult Download(int id)
    {
        var mat = _db.CourseMaterials.Find(id);
        if (mat == null || string.IsNullOrEmpty(mat.FilePath)) return NotFound();
        var path = Path.Combine(_env.WebRootPath, "uploads", "materials", mat.FilePath);
        if (!System.IO.File.Exists(path)) return NotFound();
        return File(System.IO.File.ReadAllBytes(path), "application/octet-stream", mat.FilePath);
    }

    // Upload - only Admin can create materials
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View(new CourseMaterial());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CourseMaterial model, IFormFile file)
    {
        if (file != null)
        {
            var uploads = Path.Combine(_env.WebRootPath, "uploads", "materials");
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            using (var stream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                await file.CopyToAsync(stream);
            model.FilePath = fileName;
        }

        // auditing
        model.CreatedBy = User.Identity?.Name;
        model.CreatedAt = DateTime.Now;
        model.UploadDate = DateTime.Now;

        _db.CourseMaterials.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    // Assignment list for a given subject/course (optional subjectId param)
    [Authorize]
    public async Task<IActionResult> Assignments(int? subjectId)
    {
        try
        {
            var q = _db.Assignments.AsQueryable();
            if (subjectId.HasValue)
                q = q.Where(a => a.SubjectId == subjectId.Value);
            var list = await q.OrderByDescending(a => a.CreatedAt).ToListAsync();
            return View(list ?? new List<Assignment>());
        }
        catch
        {
            return View(new List<Assignment>());
        }
    }

    [Authorize]
    public async Task<IActionResult> Submit(int id)
    {
        var assignment = await _db.Assignments.FindAsync(id);
        if (assignment == null) return NotFound();
        return View(assignment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Submit(int id, IFormFile upload)
    {
        var assignment = await _db.Assignments.FindAsync(id);
        if (assignment == null) return NotFound();
        if (upload == null || upload.Length == 0)
        {
            ModelState.AddModelError("", "Please select a file.");
            return View(assignment);
        }

        var uploads = Path.Combine(_env.WebRootPath, "uploads", "assignments");
        if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(upload.FileName);
        var fullPath = Path.Combine(uploads, fileName);
        using (var stream = new FileStream(fullPath, FileMode.Create))
            await upload.CopyToAsync(stream);

        var submission = new Submission
        {
            AssignmentId = id,
            StudentId = User.Identity?.Name,
            FilePath = fileName,
            SubmittedAt = DateTime.Now
        };
        _db.Submissions.Add(submission);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Assignments));
    }
}
