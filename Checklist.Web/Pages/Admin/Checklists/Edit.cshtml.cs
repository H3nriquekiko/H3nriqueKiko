using Checklist.Web.Data;
using Checklist.Web.Models;
using ChecklistEntity = Checklist.Web.Models.Checklist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Checklist.Web.Pages.Admin.Checklists;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;

    public EditModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public ChecklistEntity Checklist { get; set; } = new();

    public bool IsNew => Checklist.Id == 0;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            Checklist = new ChecklistEntity();
            return Page();
        }

        var checklist = await _db.Checklists.FindAsync(id.Value);
        if (checklist is null)
        {
            return NotFound();
        }

        Checklist = checklist;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Checklist.Id == 0)
        {
            _db.Checklists.Add(Checklist);
        }
        else
        {
            _db.Entry(Checklist).State = EntityState.Modified;
        }

        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
