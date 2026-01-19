using System.ComponentModel.DataAnnotations;
using Checklist.Web.Data;
using Checklist.Web.Models;
using ChecklistEntity = Checklist.Web.Models.Checklist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Checklist.Web.Pages.Admin.Checklists;

public class ItemsModel : PageModel
{
    private readonly AppDbContext _db;

    public ItemsModel(AppDbContext db)
    {
        _db = db;
    }

    public ChecklistEntity? Checklist { get; set; }

    public List<ChecklistItem> Items { get; set; } = new();

    [BindProperty]
    public NewItemInput NewItem { get; set; } = new();

    public class NewItemInput
    {
        [Required]
        [MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        public int Order { get; set; }

        public bool Required { get; set; } = true;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var checklist = await _db.Checklists.FindAsync(id);
        if (checklist is null)
        {
            return NotFound();
        }

        Checklist = checklist;
        Items = await _db.ChecklistItems
            .Where(ci => ci.ChecklistId == id)
            .OrderBy(ci => ci.Order)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync(int checklistId)
    {
        var checklist = await _db.Checklists.FindAsync(checklistId);
        if (checklist is null)
        {
            return NotFound();
        }

        Checklist = checklist;

        if (!ModelState.IsValid)
        {
            Items = await _db.ChecklistItems.Where(ci => ci.ChecklistId == checklistId).OrderBy(ci => ci.Order).ToListAsync();
            return Page();
        }

        var item = new ChecklistItem
        {
            ChecklistId = checklistId,
            Title = NewItem.Title,
            Order = NewItem.Order,
            Required = NewItem.Required,
            InputType = "Boolean"
        };

        _db.ChecklistItems.Add(item);
        await _db.SaveChangesAsync();
        return RedirectToPage(new { id = checklistId });
    }

    public async Task<IActionResult> OnPostUpdateAsync(int checklistId, int id, int order, string title, string? required)
    {
        var item = await _db.ChecklistItems.FirstOrDefaultAsync(ci => ci.Id == id && ci.ChecklistId == checklistId);
        if (item is null)
        {
            return NotFound();
        }

        item.Order = order;
        item.Title = title;
        item.Required = required == "true";

        if (string.IsNullOrWhiteSpace(item.Title) || item.Title.Length > 300)
        {
            ModelState.AddModelError(string.Empty, "Título é obrigatório e deve ter até 300 caracteres.");
            return await OnGetAsync(checklistId);
        }

        await _db.SaveChangesAsync();
        return RedirectToPage(new { id = checklistId });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int checklistId, int id)
    {
        var item = await _db.ChecklistItems.FirstOrDefaultAsync(ci => ci.Id == id && ci.ChecklistId == checklistId);
        if (item is null)
        {
            return NotFound();
        }

        _db.ChecklistItems.Remove(item);
        await _db.SaveChangesAsync();
        return RedirectToPage(new { id = checklistId });
    }
}
