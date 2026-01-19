using Checklist.Web.Data;
using Checklist.Web.Models;
using ChecklistEntity = Checklist.Web.Models.Checklist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Checklist.Web.Pages.Admin.Checklists;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public List<ChecklistEntity> Checklists { get; set; } = new();

    public async Task OnGetAsync()
    {
        Checklists = await _db.Checklists.AsNoTracking().OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<IActionResult> OnPostToggleAsync(int id)
    {
        var checklist = await _db.Checklists.FindAsync(id);
        if (checklist is null)
        {
            return NotFound();
        }

        checklist.IsActive = !checklist.IsActive;
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }
}
