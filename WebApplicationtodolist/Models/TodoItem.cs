

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

public class TodoItem
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; }

    public string Description { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime Deadline { get; set; } = DateTime.Now.AddDays(1);

    public string? FilePath { get; set; } // مسار الملف

    // Multi-tenant-ish: associate a task with a user
    public string? UserId { get; set; }

    // navigation to identity user (optional if Identity is configured)
    public IdentityUser? User { get; set; }
}
