using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationtodolist.Models
{
    public class Assignment
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        // optional SubjectId to link to subject/ course grouping
        public int? SubjectId { get; set; }

        // optional uploaded file for assignment (e.g., instructions)
        public string? FilePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
