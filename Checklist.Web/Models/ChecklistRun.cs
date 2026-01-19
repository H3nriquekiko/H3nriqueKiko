using System.ComponentModel.DataAnnotations;

namespace Checklist.Web.Models;

public class ChecklistRun
{
    public int Id { get; set; }

    [Required]
    public int ChecklistId { get; set; }

    public Checklist? Checklist { get; set; }

    public DateOnly DateRef { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? FinishedAt { get; set; }

    [Required]
    public string Status { get; set; } = "Open";

    public List<ChecklistRunItem> Items { get; set; } = new();
}
