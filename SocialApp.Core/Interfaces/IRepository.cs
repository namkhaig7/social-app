namespace SocialApp.Core.Interfaces;

/// <summary>Basic CRUD contract used by every repository (Repository pattern).</summary>
public interface IRepository<T>
{
    void Add(T item);
    T? GetById(int id);
    IReadOnlyList<T> GetAll();
    void Delete(int id);
}
