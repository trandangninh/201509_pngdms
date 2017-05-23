using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using RepositoryPattern;
using Utils;

namespace Service
{
    public interface IBaseService<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id);
        Task InsertAsync(T enity);

        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);

        Task<IPagedList<T>> GetAllAsync(int pageIndex = 0, int pageSize = int.MaxValue);
    }
}
