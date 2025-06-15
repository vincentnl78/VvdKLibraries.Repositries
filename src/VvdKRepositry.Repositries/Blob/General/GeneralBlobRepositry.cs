using VvdKRepositry.Repositries.Blob.Base;
using VvdKRepositry.Repositries.Contracts.Blob.General;

namespace VvdKRepositry.Repositries.Blob.General;

public abstract class GeneralBlobRepositry(IGeneralBlobPersistence persistence):BaseBlobRepositryWithCreationNotifers(persistence)
{
}