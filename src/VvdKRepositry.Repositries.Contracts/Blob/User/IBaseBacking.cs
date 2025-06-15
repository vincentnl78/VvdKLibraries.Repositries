namespace VvdKRepositry.Repositries.Contracts.Blob.User;

public interface IBaseBacking<TInterface> : ILoadable, IDirtyable
{
    
}

public interface ILoadable
{
    Task LoadAsync();
    bool IsLoaded { get; }
}

public interface IDirtyable
{
    public bool Dirty { get; set; }
}