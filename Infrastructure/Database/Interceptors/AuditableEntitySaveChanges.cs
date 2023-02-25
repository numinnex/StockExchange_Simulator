using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Database.Interceptors;

public sealed class AuditableEntitySaveChanges : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntites(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        UpdateEntites(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    public void UpdateEntites(DbContext? ctx)
    {
        if (ctx is null) 
            return;
        IterateOverEntitiesAndUpdate(ctx);
    }

    private static void IterateOverEntitiesAndUpdate(DbContext ctx)
    {
        foreach (var entry in ctx.ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State is EntityState.Added)
            {
                //TODO Create ICurrentUserService, ICurrentDateService
                entry.Entity.CreatedBy = null;
                entry.Entity.Created = DateTime.Now;
            }

            if (entry.State is EntityState.Added || entry.State is EntityState.Modified ||
                entry.HasChangedOwnedEntities())
            {
                //Here aswell
                entry.Entity.ModifiedBy = null;
                entry.Entity.Modified = null;
            }
        }
    }
}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry is not null
            && r.TargetEntry.Metadata.IsOwned()
            && (r.TargetEntry.State is EntityState.Added ||
                r.TargetEntry.State is EntityState.Added));
}
