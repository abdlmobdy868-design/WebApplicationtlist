using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebApplicationtodolist.Data;
using WebApplicationtodolist.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

[Authorize]
public class SubmissionsController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public SubmissionsController(AppDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // Student: create or edit submission for an assignment
    public IActionResult Create(int assignmentId)
    {
        var model = new Submission { AssignmentId = assignmentId };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Submission s, IFormFile upload)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.Identity?.Name;
        s.StudentId = userId;
        s.SubmittedAt = DateTime.UtcNow;

        // handle file upload if present
        if (upload != null && upload.Length > 0)
        {
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "submissions");
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(upload.FileName);
            var filePath = Path.Combine(uploads, fileName);
            using (var stream = System.IO.File.Create(filePath))
            {
                await upload.CopyToAsync(stream);
            }
            s.FilePath = Path.Combine("uploads", "submissions", fileName).Replace('\\', '/');
        }

        // if exists, update instead
        var existing = _db.Submissions.FirstOrDefault(x => x.AssignmentId == s.AssignmentId && x.StudentId == s.StudentId);
        if (existing != null)
        {
            if (!string.IsNullOrEmpty(s.FilePath)) existing.FilePath = s.FilePath;
            existing.SubmittedAt = DateTime.UtcNow;
            _db.Submissions.Update(existing);
        }
        else
        {
            _db.Submissions.Add(s);
        }

        await _db.SaveChangesAsync();
        return RedirectToAction("Details", "Assignments", new { id = s.AssignmentId });
    }

    // Instructor: view submissions for an assignment
    public IActionResult Index(int assignmentId)
    {
        var list = _db.Submissions.Where(s => s.AssignmentId == assignmentId).ToList();
        ViewBag.AssignmentId = assignmentId;
        return View(list);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Grade(int id, int? grade, string? feedback)
    {
        var s = _db.Submissions.Find(id);
        if (s == null) return NotFound();
        s.Grade = grade;
        s.Feedback = feedback;
        _db.Submissions.Update(s);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { assignmentId = s.AssignmentId });
    }
}
