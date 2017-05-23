using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Domain;

namespace Service.SupplyChain
{
    public interface ISupplyChainDDSService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<SupplyChainDDS> GetSupplyChainDDSById(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<SupplyChainDDS>> GetAllSupplyChainDDSs();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createdDate"></param>
        /// <returns></returns>
        Task<List<SupplyChainDDS>> GetSupplyChainDDSByDate(DateTime createdDate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measureCode"></param>
        /// <param name="createdDate"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        SupplyChainDDS GetSupplyChainDDSMeasureCodeAndDateAndType(string measureCode, DateTime createdDate, int type);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SupplyChainDDS"></param>
        /// <returns></returns>
        Task CreateAsync(SupplyChainDDS SupplyChainDDS);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SupplyChainDDS"></param>
        /// <returns></returns>
        Task UpdateAsync(SupplyChainDDS SupplyChainDDS);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SupplyChainDDS"></param>
        /// <returns></returns>
        Task DeleteAsync(SupplyChainDDS SupplyChainDDS);

    }
}
