using System.ComponentModel.DataAnnotations;

namespace Checklist.Web.Models;

public class ChecklistItem
{
    public int Id { get; set; }

    [Required]
    public int ChecklistId { get; set; }

    public Checklist? Checklist { get; set; }

    public int Order { get; set; }

    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    public bool Required { get; set; } = true;

    [Required]
    public string InputType { get; set; } = "Boolean";
}
