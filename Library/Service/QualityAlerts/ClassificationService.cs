using System;
using System.Linq;
using Entities.Domain.Classifications;
using RepositoryPattern.Repositories;
using System.Threading.Tasks;
using Utils.Caching;
using System.Data.Entity;
using Service.FoundByFunctions;
using Entities.Domain.FoundByFunction;
using Utils;

namespace Service.QualityAlerts
{
    public partial class ClassificationService : BaseService<Classification>, IClassificationService
    {
        protected override string PatternKey
        {
            get { return "PG.classification."; }
        }

        #region constant for cache

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : classificatonCode
        /// </remarks>
        private const string CLASSIFICATION_BY_CODE = "PG.classification.bycode-{0}";

        #endregion

        private readonly IRepositoryAsync<Classification> _classificationRepositoryAsync;
        private readonly IRepositoryAsync<FoundByFunction> _foundByFunctionRepositoryAsync;
        private readonly ICacheManager _cacheManager; 

        public ClassificationService(IRepositoryAsync<Classification> classificationRepositoryAsync,ICacheManager cacheManager, IRepositoryAsync<FoundByFunction> foundByFunctionRepositoryAsync)
            : base(classificationRepositoryAsync, cacheManager)
        {
            this._classificationRepositoryAsync = classificationRepositoryAsync;
            this._cacheManager = cacheManager;
            _foundByFunctionRepositoryAsync = foundByFunctionRepositoryAsync;
        }

        public Task<Classification> GetClassificationByClassificationCode(string classificationCode)
        {
            if (String.IsNullOrEmpty(classificationCode))
                return null;
            var key = string.Format(CLASSIFICATION_BY_CODE, classificationCode);
            return _cacheManager.Get(key, () =>
                _classificationRepositoryAsync.Table.FirstOrDefaultAsync(c => c.Code == classificationCode));
        }
        public IPagedList<ClassificationFullObject> GetList(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            //var res = _classificationRepositoryAsync.Table.GroupJoin(_foundByFunctionRepositoryAsync.Table,
            //    x => x.FoundByFunctionId, y => y.Id, (x, y) => new { c = x, f = y })
            //    .SelectMany(x => x.f.DefaultIfEmpty(), (x, y) => new { classi = x.c, func = y })
            //    .Select(x => new ClassificationFullObject
            //    {
            //        Id = x.classi.Id,
            //        Code = x.classi.Code,
            //        Name = x.classi.Name,
            //        Description = x.classi.Description,
            //        Severity = x.classi.Severity,
            //        Dectability = x.classi.Dectability,
            //        FoundByFunctionId = (int?)x.func.Id,
            //        FoundByFunctionName = x.func.Name
            //    });

            var res = from list1 in _classificationRepositoryAsync.Table
                      join list2 in _foundByFunctionRepositoryAsync.Table on list1.FoundByFunctionId equals list2.Id into foundByTemp
                      from foundBy in foundByTemp.DefaultIfEmpty()
                      select new ClassificationFullObject
                      {
                          Id = list1.Id,
                          Code = list1.Code,
                          Name = list1.Name,
                          Description = list1.Description,
                          Severity = list1.Severity,
                          Dectability = list1.Dectability,
                          FoundByFunctionId = (int?)foundBy.Id,
                          FoundByFunctionName = foundBy.Name
                      };
            return new PagedList<ClassificationFullObject>(res.OrderBy(x=>x.Name), pageIndex, pageSize);
        }
    }
}
