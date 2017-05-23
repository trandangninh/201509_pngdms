using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Entities.Domain;
using RepositoryPattern.Repositories;
using Utils.Caching;

namespace Service.Common
{
    public class ProductPlanningService : IProductPlanningService
    {
        #region constant for cache
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : id
        /// </remarks>
        private const string PRODUCT_PLANNING_BY_ID_KEY = "PG.productplanning.byid-{0}";


        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : id
        /// </remarks>
        private const string PRODUCT_PLANNING_BY_DATE_SHIFT_LINE_KEY = "PG.productplanning.bydate-{0}-shift-{1}-line-{2}";

        
        /// <summary>
        /// Key for caching
        /// </summary>
        private const string PRODUCT_PLANNING_KEY = "PG.productplanning.all";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string PRODUCT_PLANNING_PATTERN = "PG.productplanning.";

       
        #endregion

        private readonly IRepositoryAsync<ProductPlanning> _productPlanningRepositoryAsync;

        private readonly ICacheManager _cacheManager;


        public ProductPlanningService(IRepositoryAsync<ProductPlanning> productPlanningRepositoryAsync, 
            ICacheManager cacheManager)
        {
            _productPlanningRepositoryAsync = productPlanningRepositoryAsync;
            _cacheManager = cacheManager;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="productPlanningId"></param>
        /// <returns></returns>
        public Task<ProductPlanning> GetProductPlanningByIdAsync(int productPlanningId)
        {
            var key = string.Format(PRODUCT_PLANNING_BY_ID_KEY, productPlanningId);
            return _cacheManager.Get(key, 
                () => 
                    _productPlanningRepositoryAsync.GetByIdAsync(productPlanningId)
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<List<ProductPlanning>> GetAllProductPlanning()
        {
            var key = string.Format(PRODUCT_PLANNING_BY_ID_KEY, PRODUCT_PLANNING_KEY);
            return _cacheManager.Get(key, () => 
                _productPlanningRepositoryAsync.Table.ToListAsync());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="shiftType"></param>
        /// <param name="lineType"></param>
        /// <returns></returns>
        public Task<ProductPlanning> GetProductPlanningByDateAndShiftAndLine(DateTime beginDate,
            PlanShiftHardCodeType shiftType, PlanLineHardCodeType lineType)
        {
            var key = string.Format(PRODUCT_PLANNING_BY_DATE_SHIFT_LINE_KEY, beginDate.Date.ToShortDateString(), shiftType, lineType);
            return _cacheManager.Get(key, () =>
            {
                var startTime = new DateTime(beginDate.Year, beginDate.Month, beginDate.Day, 0, 0, 0);
                var endTime = startTime.AddDays(1);

                return _productPlanningRepositoryAsync.Table.FirstOrDefaultAsync(x => x.CreatedDate < endTime && x.CreatedDate >= startTime && x.Shift == shiftType && x.Line == lineType);
            });
        }

        public Task CreateAsync(ProductPlanning productPlanning)
        {
            _cacheManager.RemoveByPattern(PRODUCT_PLANNING_PATTERN);

            return _productPlanningRepositoryAsync.InsertAsync(productPlanning);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productPlanning"></param>
        /// <returns></returns>
        public Task UpdateAsync(ProductPlanning productPlanning)
        {
            _cacheManager.RemoveByPattern(PRODUCT_PLANNING_PATTERN);
            return _productPlanningRepositoryAsync.UpdateAsync(productPlanning);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productPlanning"></param>
        /// <returns></returns>
        public Task DeleteAsync(ProductPlanning productPlanning)
        {
            _cacheManager.RemoveByPattern(PRODUCT_PLANNING_PATTERN);
            return _productPlanningRepositoryAsync.DeleteAsync(productPlanning);
        }
    }
}
