namespace VvdKRepositry.Repositries.Contracts.Blob.User;

public interface IBaseBacking
{
    Task LoadAsync();
    bool IsLoaded { get; }
    public bool Dirty { get; set; }
}