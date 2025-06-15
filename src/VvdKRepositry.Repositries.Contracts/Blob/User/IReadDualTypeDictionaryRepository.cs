namespace VvdKRepositry.Repositries.Contracts.Blob.User;

public interface IReadDualTypeDictionaryRepository<T,T1,T2>:
    IReadDictionaryRepository<int,T> 
    where T : class, IId<int>
    where T1 : class,T,IId<int>
    where T2 : class,T,IId<int>
{
    IReadOnlyDictionary<int,T1> T1All { get; }
    IReadOnlyDictionary<int,T2> T2All { get; }
}