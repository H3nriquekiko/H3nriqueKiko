using Checklist.Web.Models;
using ChecklistEntity = Checklist.Web.Models.Checklist;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Checklist.Web.Data;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<ChecklistEntity> Checklists => Set<ChecklistEntity>();
    public DbSet<ChecklistItem> ChecklistItems => Set<ChecklistItem>();
    public DbSet<ChecklistRun> ChecklistRuns => Set<ChecklistRun>();
    public DbSet<ChecklistRunItem> ChecklistRunItems => Set<ChecklistRunItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ChecklistEntity>(entity =>
        {
            entity.Property(c => c.IsActive).HasDefaultValue(true);
            entity.Property(c => c.Frequency).HasDefaultValue("Daily");
        });

        builder.Entity<ChecklistItem>(entity =>
        {
            entity.HasIndex(ci => new { ci.ChecklistId, ci.Order });
            entity.Property(ci => ci.Required).HasDefaultValue(true);
            entity.Property(ci => ci.InputType).HasDefaultValue("Boolean");
        });

        builder.Entity<ChecklistRun>(entity =>
        {
            entity.HasIndex(cr => new { cr.ChecklistId, cr.DateRef }).IsUnique();
            entity.Property(cr => cr.Status).HasDefaultValue("Open");
        });

        builder.Entity<ChecklistRunItem>(entity =>
        {
            entity.HasIndex(cri => new { cri.RunId, cri.ItemId }).IsUnique();
            entity.Property(cri => cri.Status).HasDefaultValue("Pending");
        });
    }
}
