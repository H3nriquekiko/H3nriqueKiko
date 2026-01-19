using Checklist.Web.Data;
using Checklist.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Checklist.Web.Pages;

public class RunModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public RunModel(AppDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public ChecklistRun? Run { get; set; }

    public async Task<IActionResult> OnGetAsync(int runId)
    {
        Run = await LoadRunAsync(runId);
        if (Run is null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostUpdateAsync(int runId, int itemId, string status, string? notes)
    {
        Run = await LoadRunAsync(runId);
        if (Run is null)
        {
            return NotFound();
        }

        if (Run.Status == "Done")
        {
            return RedirectToPage(new { runId });
        }

        var runItem = Run.Items.FirstOrDefault(i => i.Id == itemId);
        if (runItem is null)
        {
            return NotFound();
        }

        if (status == "Fail" && string.IsNullOrWhiteSpace(notes))
        {
            ModelState.AddModelError(string.Empty, "Notas são obrigatórias para falha.");
            return Page();
        }

        if (status != "Ok" && status != "Fail")
        {
            ModelState.AddModelError(string.Empty, "Status inválido.");
            return Page();
        }

        runItem.Status = status;
        runItem.Notes = notes;
        runItem.FilledAt = DateTime.UtcNow;
        runItem.FilledByUserId = _userManager.GetUserId(User);

        await _db.SaveChangesAsync();
        return RedirectToPage(new { runId });
    }

    public async Task<IActionResult> OnPostFinishAsync(int runId)
    {
        Run = await LoadRunAsync(runId);
        if (Run is null)
        {
            return NotFound();
        }

        if (Run.Status == "Done")
        {
            return RedirectToPage(new { runId });
        }

        var pendingRequired = Run.Items.Any(i => i.Item?.Required == true && i.Status == "Pending");
        if (pendingRequired)
        {
            ModelState.AddModelError(string.Empty, "Finalize todos os itens obrigatórios antes de concluir.");
            return Page();
        }

        Run.Status = "Done";
        Run.FinishedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return RedirectToPage(new { runId });
    }

    private async Task<ChecklistRun?> LoadRunAsync(int runId)
    {
        return await _db.ChecklistRuns
            .Include(r => r.Checklist)
            .Include(r => r.Items)
                .ThenInclude(ri => ri.Item)
            .FirstOrDefaultAsync(r => r.Id == runId);
    }
}
