using System.ComponentModel.DataAnnotations;

public class CourseMaterial
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    public string? Description { get; set; }

    public string? FilePath { get; set; }

    public DateTime UploadDate { get; set; } = DateTime.Now;

    public string? Category { get; set; }
    // auditing
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
