using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationtodolist.Models
{
    public class Submission
    {
        public int Id { get; set; }

        [Required]
        public int AssignmentId { get; set; }

        // Student identifier (string) - link to Identity user id
        [Required]
        public string StudentId { get; set; }

        // File uploaded path under wwwroot/uploads
        public string FilePath { get; set; }

        public int? Grade { get; set; }

        public string? Feedback { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
