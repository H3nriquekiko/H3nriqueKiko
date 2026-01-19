using Checklist.Web.Data;
using Checklist.Web.Models;
using ChecklistEntity = Checklist.Web.Models.Checklist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Checklist.Web.Pages;

public class HistoryModel : PageModel
{
    private readonly AppDbContext _db;

    public HistoryModel(AppDbContext db)
    {
        _db = db;
    }

    public List<ChecklistRun> Runs { get; set; } = new();

    public List<ChecklistEntity> AvailableChecklists { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int? ChecklistId { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? StartDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? EndDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Status { get; set; } = "All";

    public async Task OnGetAsync()
    {
        AvailableChecklists = await _db.Checklists.AsNoTracking().OrderBy(c => c.Name).ToListAsync();

        var query = _db.ChecklistRuns
            .Include(r => r.Checklist)
            .AsNoTracking()
            .AsQueryable();

        if (ChecklistId.HasValue)
        {
            query = query.Where(r => r.ChecklistId == ChecklistId.Value);
        }

        if (StartDate.HasValue)
        {
            var start = DateOnly.FromDateTime(StartDate.Value);
            query = query.Where(r => r.DateRef >= start);
        }

        if (EndDate.HasValue)
        {
            var end = DateOnly.FromDateTime(EndDate.Value);
            query = query.Where(r => r.DateRef <= end);
        }

        if (Status is "Open" or "Done")
        {
            query = query.Where(r => r.Status == Status);
        }

        Runs = await query.OrderByDescending(r => r.DateRef).ThenBy(r => r.Checklist!.Name).ToListAsync();
    }
}
