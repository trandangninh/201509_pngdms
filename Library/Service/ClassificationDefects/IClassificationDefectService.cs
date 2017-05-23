using Entities.Domain.ClassificationDefects;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace Service.ClassificationDefects
{
    public interface IClassificationDefectService : IBaseService<ClassificationDefect>
    {
        /// <summary>
        /// Get all ClassificationDefect
        /// </summary>
        /// <param name="searchByClassificationDefectName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>paged list ClassificationDefect</returns>
        Task<IPagedList<ClassificationDefect>> GetAllClassificationDefectAsync(string searchByClassificationDefectName = null, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// check name of ClassificationDefect existed or not
        /// </summary>
        /// <param name="name"></param>
        /// <returns>true or false</returns>
        Task<bool> CheckNameHasExisted(string name);

        /// <summary>
        /// get ClassificationDefect by name
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        Task<ClassificationDefect> GetClassificationDefectByNameAsync(string name);

        /// <summary>
        /// delete list ClassificationDefect
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        Task DeleteClassificationDefectsAsync(List<int> listId);
    }
}
