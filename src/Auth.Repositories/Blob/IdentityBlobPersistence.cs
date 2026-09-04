using System.Text.Json;
using Auth.Interfaces.Storage;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using VvdKRepositry.Repositries.Blob.Base;

namespace Auth.Repositories.Blob;

public class IdentityBlobPersistence(
    IAzureClientFactory<BlobServiceClient> factory,
    JsonSerializerOptions jsonSerializerOptions)
    : BaseBlobPersistence(factory.CreateClient(IIdentityBlobPersistence.StorageServiceIdentifier),
        jsonSerializerOptions), IIdentityBlobPersistence
{
}