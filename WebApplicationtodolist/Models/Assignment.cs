using System.ComponentModel.DataAnnotations;

public class Assignment
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    public string? Description { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime Deadline { get; set; } = DateTime.Now.AddDays(7);

    public string? FilePath { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
