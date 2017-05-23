using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;

namespace Service.Interface
{
    public interface ISupplyChainServiceService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<SupplyChainService> GetSupplyChainServiceById(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<SupplyChainService>> GetAllSupplyChainServices();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<SupplyChainService> GetAllSupplyChainServicesNotAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineId"></param>
        /// <param name="createdDate"></param>
        /// <returns></returns>
        Task<List<SupplyChainService>> GetSupplyChainServiceByDate(DateTime createdDate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measureCode"></param>
        /// <param name="createdDate"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        SupplyChainService GetSupplyChainServiceMeasureCodeAndDateAndType(string measureCode, DateTime createdDate, int type);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SupplyChainService"></param>
        /// <returns></returns>
        Task CreateAsync(SupplyChainService SupplyChainService);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SupplyChainService"></param>
        /// <returns></returns>
        Task UpdateAsync(SupplyChainService SupplyChainService);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SupplyChainService"></param>
        /// <returns></returns>
        Task DeleteAsync(SupplyChainService SupplyChainService);

    }
}
