using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Domain;

namespace Service.Departments
{
    public interface IMeasureSupplyChainService
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MeasureSupplyChain> GetMeasureSupplyChainById(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<MeasureSupplyChain> GetMeasureSupplyChainByCode(string code);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<MeasureSupplyChain>> GetAllMeasureSupplyChains();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dmsCode"></param>
        /// <returns></returns>
        Task<List<MeasureSupplyChain>> GetMeasureSupplyChainByDmsCode(string dmsCode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dmsCode"></param>
        /// <returns></returns>
        Task<MeasureSupplyChain> GetMeasureSupplyChainByDmsCodeAndMeasureCode(string dmsCode, string code);
       /// <summary>
       /// 
       /// </summary>
       /// <param name="measureSupplyChain"></param>
       /// <returns></returns>
        Task CreateAsync(MeasureSupplyChain measureSupplyChain);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measureSupplyChain"></param>
        /// <returns></returns>
        Task UpdateAsync(MeasureSupplyChain measureSupplyChain);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measureSupplyChain"></param>
        /// <returns></returns>
        Task DeleteAsync(MeasureSupplyChain measureSupplyChain);


      

    }
}
