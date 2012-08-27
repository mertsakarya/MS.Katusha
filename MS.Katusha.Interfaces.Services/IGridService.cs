using System.Collections.Generic;

namespace MS.Katusha.Interfaces.Services
{
    public interface IGridService<T>
    {
        IList<T> GetAll(out int total, int page, int pageSize);
        void Add(T entity);
        T GetById(long id);
        void Update(T entity);
        void Delete(T entity);
    }
}
