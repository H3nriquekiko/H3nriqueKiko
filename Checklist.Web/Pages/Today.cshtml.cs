using Checklist.Web.Data;
using Checklist.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Checklist.Web.Pages;

public class TodayModel : PageModel
{
    private readonly AppDbContext _db;

    public TodayModel(AppDbContext db)
    {
        _db = db;
    }

    public List<Checklist> Checklists { get; set; } = new();

    public DateOnly TodayLocal { get; set; }

    public async Task OnGetAsync()
    {
        TodayLocal = GetLocalDate();
        Checklists = await _db.Checklists
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostRunAsync(int id)
    {
        var checklist = await _db.Checklists
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (checklist is null)
        {
            return NotFound();
        }

        var dateRef = GetLocalDate();
        var run = await _db.ChecklistRuns
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.ChecklistId == id && r.DateRef == dateRef);

        if (run is null)
        {
            run = new ChecklistRun
            {
                ChecklistId = id,
                DateRef = dateRef,
                StartedAt = DateTime.UtcNow,
                Status = "Open"
            };

            foreach (var item in checklist.Items.OrderBy(i => i.Order))
            {
                run.Items.Add(new ChecklistRunItem
                {
                    ItemId = item.Id,
                    Status = "Pending"
                });
            }

            _db.ChecklistRuns.Add(run);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage("/Run", new { runId = run.Id });
    }

    private static DateOnly GetLocalDate()
    {
        var timeZone = GetSaoPauloTimeZone();
        var local = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
        return DateOnly.FromDateTime(local);
    }

    private static TimeZoneInfo GetSaoPauloTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.Local;
        }
    }
}
