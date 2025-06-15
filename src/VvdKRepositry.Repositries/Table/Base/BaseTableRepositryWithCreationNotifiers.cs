using Serilog;
using VvdKRepositry.Repositries.Contracts.Notifications;
using VvdKRepositry.Repositries.Contracts.Notifications.Creation;
using VvdKRepositry.Repositries.Contracts.Table.Base;

namespace VvdKRepositry.Repositries.Table.Base;

public abstract class BaseTableRepositryWithCreationNotifiers(IBaseTablePersistence persistence)
    : BaseTableRepositry(persistence), IGeneralPersistenceCreationNotifications
{
    private readonly IBaseTablePersistence _persistence = persistence;

    public async Task Handle(CreateGeneralPersistenceSetupNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            await _persistence.InitializeAsync(TableName);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating {TableName}", TableName);
        }
        
    }

    public async Task Handle(DeleteGeneralPersistenceNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            await _persistence.DeleteTableAsync(TableName);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting {TableName}", TableName);
        }
    }
}