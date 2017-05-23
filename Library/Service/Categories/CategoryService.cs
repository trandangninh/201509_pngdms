using Entities.Domain.Categories;
using RepositoryPattern.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Utils.Caching;
using System.Data.Entity;

namespace Service.Categories
{
    public class CategoryService : BaseService<Category>, ICategoryService
    {
        /// <summary>
        /// Key for caching
        /// </summary>
        /// </remarks>
        private const string CATEGORY_LISTPAGED_KEY = "PG.Category.ListPaged-{0}-{1}-{2}";

        private const string CATEGORY_BY_NAME = "PG.Category.ByName-{0}";

        protected override string PatternKey
        {
            get { return "PG.Category."; }
        }

        private readonly ICacheManager _cacheManager;
        private readonly IRepositoryAsync<Category> _categoryRepositoryAsync;

        public CategoryService(ICacheManager cacheManager,
            IRepositoryAsync<Category> categoryRepositoryAsync)
            : base(categoryRepositoryAsync,cacheManager)
        {
            _categoryRepositoryAsync = categoryRepositoryAsync;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// Get all category
        /// </summary>
        /// <returns>paged list category</returns>
        public Task<IPagedList<Category>> GetAllCategoryAsync(string searchByCategoryName = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = string.Format(CATEGORY_LISTPAGED_KEY, searchByCategoryName, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
                {
                    var query = _categoryRepositoryAsync.Table.AsQueryable();

                    if (!string.IsNullOrEmpty(searchByCategoryName))
                        query = query.Where(s => s.Name.Contains(searchByCategoryName));

                    //defaulf order by name
                    query = query.OrderBy(c => c.Name);
                    return Task.FromResult(new PagedList<Category>(query, pageIndex, pageSize) as IPagedList<Category>);
                }
            );
        }

        /// <summary>
        /// check name of category existed or not
        /// </summary>
        /// <param name="name"></param>
        /// <returns>true or false</returns>
        public Task<bool> CheckNameHasExisted(string name)
        {
            return Task.FromResult(_categoryRepositoryAsync.Table.Any(c => c.Name == name));
        }

        /// <summary>
        /// get category by name
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public Task<Category> GetCategoryByNameAsync(string name)
        {
            var key = string.Format(CATEGORY_BY_NAME, name);
            return _cacheManager.Get(key, () => _categoryRepositoryAsync.Table.FirstOrDefaultAsync(c => c.Name == name));
        }

        /// <summary>
        /// delete list categories
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        public Task DeleteCategoriesAsync(List<int> listId)
        {
            if (listId == null)
                throw new ArgumentNullException("listId is null");
            var deletedCategories = _categoryRepositoryAsync.Table.Where(i => listId.Contains(i.Id));
            _cacheManager.RemoveByPattern(PatternKey);
            return _categoryRepositoryAsync.DeleteAsync(deletedCategories);
        }
    }
}
