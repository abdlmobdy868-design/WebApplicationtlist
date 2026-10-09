using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebApplicationtodolist.Data;
using WebApplicationtodolist.Models;
using System.Security.Claims;

[Authorize]
public class SubmissionsController : Controller
{
    private readonly AppDbContext _db;

    public SubmissionsController(AppDbContext db)
    {
        _db = db;
    }

    // Student: create or edit submission for an assignment
    public IActionResult Create(int assignmentId)
    {
        var model = new Submission { AssignmentId = assignmentId };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Submission s)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.Identity?.Name;
        s.StudentId = userId;
        s.SubmittedAt = DateTime.Now;

        // if exists, update instead
        var existing = _db.Submissions.FirstOrDefault(x => x.AssignmentId == s.AssignmentId && x.StudentId == s.StudentId);
        if (existing != null)
        {
            existing.GitHubLink = s.GitHubLink;
            existing.Note = s.Note;
            existing.SubmittedAt = DateTime.Now;
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
