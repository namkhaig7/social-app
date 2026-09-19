namespace SocialApp.Core.Interfaces;

//Yum hadgalna
public interface IRepository<T>
{
    void Add(T item);
    T? GetById(int id);
    IReadOnlyList<T> GetAll();
    void Delete(int id);

    // Daraachiin id-g repository oorroo ogno (DB mou sanah oid hamaaralgui)
    int NextId();
}
