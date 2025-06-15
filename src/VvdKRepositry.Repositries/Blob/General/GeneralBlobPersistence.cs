using System.Text.Json;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using VvdKRepositry.Repositries.Blob.Base;
using VvdKRepositry.Repositries.Contracts.Blob.General;

namespace VvdKRepositry.Repositries.Blob.General;

public class GeneralBlobPersistence(
    IAzureClientFactory<BlobServiceClient> factory,
    JsonSerializerOptions jsonSerializerOptions)
    : BaseBlobPersistence(factory.CreateClient(IGeneralBlobPersistence.StorageServiceIdentifier), jsonSerializerOptions), IGeneralBlobPersistence
{
}