using RepositoryPattern.Repositories;
using System.Linq;
using System.Threading.Tasks;
using Utils;
using Utils.Caching;
using Entities.Domain.ClassificationDefects;
using System.Data.Entity;
using System;
using System.Collections.Generic;

namespace Service.ClassificationDefects
{
    public class ClassificationDefectService : BaseService<ClassificationDefect>, IClassificationDefectService
    {
        /// <summary>
        /// Key for caching
        /// </summary>
        /// </remarks>
        private const string CLASSIFICATIONDEFECT_LISTPAGED_KEY = "PG.ClassificationDefect.ListPaged-{0}-{1}-{2}";

        private const string CLASSIFICAIONDEFECT_BY_NAME = "PG.ClassificationDefect.ByName-{0}";

        protected override string PatternKey
        {
            get { return "PG.ClassificationDefect."; }
        }

        private readonly ICacheManager _cacheManager;
        private readonly IRepositoryAsync<ClassificationDefect> _classificationDefectRepositoryAsync;

        public ClassificationDefectService(ICacheManager cacheManager,
            IRepositoryAsync<ClassificationDefect> classificationDefectRepositoryAsync)
            : base(classificationDefectRepositoryAsync, cacheManager)
        {
            _classificationDefectRepositoryAsync = classificationDefectRepositoryAsync;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// Get all ClassificationDefect
        /// </summary>
        /// <returns>paged list ClassificationDefect</returns>
        public Task<IPagedList<ClassificationDefect>> GetAllClassificationDefectAsync(string searchByClassificationDefectName = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = string.Format(CLASSIFICATIONDEFECT_LISTPAGED_KEY, searchByClassificationDefectName, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {
                var query = _classificationDefectRepositoryAsync.Table.AsQueryable();

                if (!string.IsNullOrEmpty(searchByClassificationDefectName))
                    query = query.Where(s => s.Name.Contains(searchByClassificationDefectName));

                //default sort by classification defect name
                query = query.OrderBy(s => s.DisplayOrder);
                return Task.FromResult(new PagedList<ClassificationDefect>(query, pageIndex, pageSize) as IPagedList<ClassificationDefect>);
            }
            );
        }

        /// <summary>
        /// check name of ClassificationDefect existed or not
        /// </summary>
        /// <param name="name"></param>
        /// <returns>true or false</returns>
        public Task<bool> CheckNameHasExisted(string name)
        {
            return Task.FromResult(_classificationDefectRepositoryAsync.Table.Any(c => c.Name == name));
        }

        /// <summary>
        /// get ClassificationDefect by name
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public Task<ClassificationDefect> GetClassificationDefectByNameAsync(string name)
        {
            var key = string.Format(CLASSIFICAIONDEFECT_BY_NAME, name);
            return _cacheManager.Get(key, () => _classificationDefectRepositoryAsync.Table.FirstOrDefaultAsync(c => c.Name == name));
        }

        /// <summary>
        /// delete list ClassificationDefect
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        public async Task DeleteClassificationDefectsAsync(List<int> listId)
        {
            if (listId == null)
                throw new ArgumentNullException("listId is null");
            var deletedClassificationDefects = _classificationDefectRepositoryAsync.Table.Where(i => listId.Contains(i.Id)).ToList();
            _cacheManager.RemoveByPattern(PatternKey);
            foreach(var item in deletedClassificationDefects)
            {
                item.Suppliers.Clear();

                await _classificationDefectRepositoryAsync.UpdateAsync(item);
            }

            await _classificationDefectRepositoryAsync.DeleteAsync(deletedClassificationDefects);
        }
    }
}
