using AwesomeAssertions;
using VvdKRepositry.Repositries.Blob.General;
using VvdKRepositry.Repositries.Contracts.Blob.Base;
using VvdKRepositry.Repositries.Contracts.Blob.General;


namespace VvdKRepositry.Repositries.UnitTests;

public class BlobPersistenceTests()
{
    public class TestGeneralBlobRepositry(IGeneralBlobPersistence persistence) : GeneralBlobRepositry(persistence)
    {
        protected override string ContainerName => "testcontainer";
    }
    
    
    private readonly MemoryStream _dataStream = new("test"u8.ToArray());
    private IBlobRepositry _cut = null!;

    [Fact]
    public async Task DataStreamPersistence_SaveIncreasingFileName()
    {
        var fakePersistence = new FakeBlobStore();
        
        _cut = new TestGeneralBlobRepositry(fakePersistence);
        var filename = "file.txt";
        var result = await _cut.SavePotentiallyRenameImportFileAsync(_dataStream, filename, "directory");
        result.Should().Be(filename);
        result = await _cut.SavePotentiallyRenameImportFileAsync(_dataStream, filename, "directory");
        result.Should().Be("file(1).txt");
    }
}