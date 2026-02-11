namespace VvdKRepositry.Repositries.Contracts.Blob.User;

public interface IId<out T>
     
{
    T Id { get; }
}