using System;

namespace WebApplicationtodolist.Models
{
    public class AdminSubmissionViewModel
    {
        public int Id { get; set; }
        public string StudentEmail { get; set; }
        public string AssignmentTitle { get; set; }
        public string FilePath { get; set; }
        public int? Grade { get; set; }
        public string? Feedback { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
