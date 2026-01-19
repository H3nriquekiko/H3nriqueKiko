using System.ComponentModel.DataAnnotations;

namespace Checklist.Web.Models;

public class ChecklistRunItem
{
    public int Id { get; set; }

    [Required]
    public int RunId { get; set; }

    public ChecklistRun? Run { get; set; }

    [Required]
    public int ItemId { get; set; }

    public ChecklistItem? Item { get; set; }

    [Required]
    public string Status { get; set; } = "Pending";

    [MaxLength(2000)]
    public string? Notes { get; set; }

    public string? FilledByUserId { get; set; }

    public DateTime? FilledAt { get; set; }
}
