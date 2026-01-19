using System.ComponentModel.DataAnnotations;

namespace Checklist.Web.Models;

public class Checklist
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    [Required]
    public string Frequency { get; set; } = "Daily";

    public TimeOnly? DueTime { get; set; }

    public List<ChecklistItem> Items { get; set; } = new();
}
