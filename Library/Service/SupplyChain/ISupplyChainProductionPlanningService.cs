using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Domain;

namespace Service.SupplyChain
{
    public interface ISupplyChainProductionPlanningService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<SupplyChainProductionPlanning> GetSupplyChainProductionPlanningById(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<SupplyChainProductionPlanning>> GetAllSupplyChainProductionPlannings();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createdDate"></param>
        /// <returns></returns>
        Task<List<SupplyChainProductionPlanning>> GetSupplyChainProductionPlanningByDate(DateTime createdDate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measureCode"></param>
        /// <param name="createdDate"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        SupplyChainProductionPlanning GetSupplyChainProductionPlanningMeasureCodeAndDateAndType(string measureCode, DateTime createdDate, int type);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="supplyChainProductionPlanning"></param>
        /// <returns></returns>
        Task CreateAsync(SupplyChainProductionPlanning supplyChainProductionPlanning);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supplyChainProductionPlanning"></param>
        /// <returns></returns>
        Task UpdateAsync(SupplyChainProductionPlanning supplyChainProductionPlanning);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supplyChainProductionPlanning"></param>
        /// <returns></returns>
        Task DeleteAsync(SupplyChainProductionPlanning supplyChainProductionPlanning);

    }
}
