using Entities.Domain.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Service.Categories
{
    public interface ICategoryService : IBaseService<Category>
    {
        /// <summary>
        /// Get all category
        /// </summary>
        /// <returns>paged list category</returns>
        Task<IPagedList<Category>> GetAllCategoryAsync(string searchByCategoryName = null, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// check name of category existed or not
        /// </summary>
        /// <param name="name"></param>
        /// <returns>true or false</returns>
        Task<bool> CheckNameHasExisted(string name);

        /// <summary>
        /// get category by name
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        Task<Category> GetCategoryByNameAsync(string name);

        /// <summary>
        /// delete list categories
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        Task DeleteCategoriesAsync(List<int> listId);
    }
}
