using Microsoft.EntityFrameworkCore;
using SharedCore.Abstraction.Entities;
using SharedCore.Abstraction.Services;

namespace SharedCore.Infrastructure.EntityFramework;

public abstract class DbContextBase : DbContext
{
    private readonly IClock _clock;

    protected DbContextBase(DbContextOptions options, IClock clock) : base(options)
    {
        _clock = clock;
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        BeforeSaveChanges();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        BeforeSaveChanges();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        BeforeSaveChanges();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = new())
    {
        BeforeSaveChanges();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void BeforeSaveChanges()
    {
        var entries = ChangeTracker.Entries();

        foreach (var entry in entries)
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (entry.Entity is IAuditableEntity entity)
            {
                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (entry.State)
                {
                    case EntityState.Added:
                        entity.CreateDate = _clock.UtcOffsetNow;
                        entity.ModifyDate = _clock.UtcOffsetNow;
                        break;
                    case EntityState.Modified:
                        entity.ModifyDate = _clock.UtcOffsetNow;
                        break;
                }
            }

            // ReSharper disable once InvertIf
            if (entry is { Entity: ISoftDeletableEntity softDeletableEntity, State: EntityState.Deleted })
            {
                entry.State = EntityState.Modified;
                softDeletableEntity.DeleteDate = _clock.UtcOffsetNow;
            }
        }
    }
}