namespace VvdKRepositry.Repositries.Contracts.Blob.User;

public interface IId<out T>
    where T : struct
{
    T Id { get; }
}