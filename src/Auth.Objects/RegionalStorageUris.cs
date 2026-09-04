namespace Auth.Objects;

public class RegionalStorageUris
{
    public RegionalStorageUris(ApiAreas area, string blobUri, string tableUri)
    {
        TableUrls.Add(area, tableUri);
        BlobUrls.Add(area, blobUri);
    }

    public RegionalStorageUris(Dictionary<string, string> keyValues)
    {
        foreach (var value in keyValues)
        {
            switch (value.Key[2])
            {
                case 'T':
                    TableUrls.Add(Enum.Parse<ApiAreas>(value.Key[..2]), value.Value);
                    break;
                case 'B':
                    BlobUrls.Add(Enum.Parse<ApiAreas>(value.Key[..2]), value.Value);
                    break;
            }
        }
    }

    public List<ApiAreas> Areas => TableUrls.Keys.Concat(BlobUrls.Keys).Distinct().ToList();

    public Dictionary<ApiAreas, string> TableUrls { get; } = new();
    public Dictionary<ApiAreas, string> BlobUrls { get; } = new();
}