namespace VvdKRepositry.Repositries.Contracts.Blob.User;

public interface IReadWriteSingleClass<T>: IBaseBacking
{
    public T Instance { get; set; }
}