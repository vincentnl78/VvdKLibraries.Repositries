using Serilog;
using VvdKRepositry.Repositries.Contracts.Blob.Base;
using VvdKRepositry.Repositries.Contracts.Notifications;
using VvdKRepositry.Repositries.Contracts.Notifications.Creation;

namespace VvdKRepositry.Repositries.Blob.Base;

public abstract class BaseBlobRepositryWithCreationNotifers(IBaseBlobPersistence persistence)
    : BaseBlobRepositry(persistence), IGeneralPersistenceCreationNotifications
{
    private readonly IBaseBlobPersistence _persistence = persistence;

    public async Task Handle(CreateGeneralPersistenceSetupNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            await _persistence.InitializeAsync(ContainerName);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating {ContainerName}", ContainerName);
        }
        
    }

    public async Task Handle(DeleteGeneralPersistenceNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            await _persistence.DeleteContainerAsync(ContainerName);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting {ContainerName}", ContainerName);
        }
    }
}