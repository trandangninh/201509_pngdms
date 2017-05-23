using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Domain;

namespace Service.Common
{
    public interface IProductPlanningService
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productPlanningId"></param>
        /// <returns></returns>
        Task<ProductPlanning> GetProductPlanningByIdAsync(int productPlanningId);


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<ProductPlanning>> GetAllProductPlanning();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="shiftType"></param>
        /// <param name="lineType"></param>
        /// <returns></returns>
        Task<ProductPlanning> GetProductPlanningByDateAndShiftAndLine(DateTime beginDate,
            PlanShiftHardCodeType shiftType, PlanLineHardCodeType lineType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productPlanning"></param>
        /// <returns></returns>
        Task CreateAsync(ProductPlanning productPlanning);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="productPlanning"></param>
        /// <returns></returns>
        Task UpdateAsync(ProductPlanning productPlanning);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productPlanning"></param>
        /// <returns></returns>
        Task DeleteAsync(ProductPlanning productPlanning);
    }
}
