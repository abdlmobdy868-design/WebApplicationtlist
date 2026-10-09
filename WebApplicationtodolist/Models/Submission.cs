using System.ComponentModel.DataAnnotations;

public class Submission
{
    public int Id { get; set; }

    [Required]
    public int AssignmentId { get; set; }

    // Student identifier (string) - link to Identity user id
    [Required]
    public string StudentId { get; set; }

    [Required]
    [RegularExpression(@".*github\.com.*", ErrorMessage = "Link must be a GitHub URL")]
    public string GitHubLink { get; set; }

    public string? Note { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.Now;

    public int? Grade { get; set; }

    public string? Feedback { get; set; }
}
