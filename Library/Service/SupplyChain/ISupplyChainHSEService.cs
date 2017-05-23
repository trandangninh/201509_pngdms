using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Domain;

namespace Service.SupplyChain
{
    public interface ISupplyChainHSEService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<SupplyChainHSE> GetSupplyChainHSEById(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<SupplyChainHSE>> GetAllSupplyChainHSEs();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineId"></param>
        /// <param name="createdDate"></param>
        /// <returns></returns>
        Task<List<SupplyChainHSE>> GetSupplyChainHSEByDate(DateTime createdDate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measureCode"></param>
        /// <param name="createdDate"></param>
        /// <returns></returns>
        SupplyChainHSE GetSupplyChainHSEMeasureCodeAndDate(string measureCode, DateTime createdDate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supplyChainHSE"></param>
        /// <returns></returns>
        Task CreateAsync(SupplyChainHSE supplyChainHSE);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supplyChainHSE"></param>
        /// <returns></returns>
        Task UpdateAsync(SupplyChainHSE supplyChainHSE);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supplyChainHSE"></param>
        /// <returns></returns>
        Task DeleteAsync(SupplyChainHSE supplyChainHSE);

    }
}
