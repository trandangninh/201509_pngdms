using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Domain;

namespace Service.SupplyChain
{
    public interface ISupplyChainMPSAService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<SupplyChainMPSA> GetSupplyChainMPSAById(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<SupplyChainMPSA>> GetAllSupplyChainMPSAs();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineId"></param>
        /// <param name="createdDate"></param>
        /// <returns></returns>
        Task<List<SupplyChainMPSA>> GetSupplyChainMPSAByDate(DateTime createdDate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measureCode"></param>
        /// <param name="createdDate"></param>
        /// <returns></returns>
        SupplyChainMPSA GetSupplyChainMPSAMeasureCodeAndDate(string measureCode, DateTime createdDate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supplyChainMPSA"></param>
        /// <returns></returns>
        Task CreateAsync(SupplyChainMPSA supplyChainMPSA);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supplyChainMPSA"></param>
        /// <returns></returns>
        Task UpdateAsync(SupplyChainMPSA supplyChainMPSA);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supplyChainMPSA"></param>
        /// <returns></returns>
        Task DeleteAsync(SupplyChainMPSA supplyChainMPSA);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="SupplyChainMPSAId"></param>
        /// <returns></returns>
        Task<List<UserInSupplyChainMPSA>> GetUserInSupplyChainMPSA(int SupplyChainMPSAId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SupplyChainMPSAId"></param>
        /// <returns></returns>
        List<string> GetUserNameInSupplyChainMPSA(int SupplyChainMPSAId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SupplyChainMPSAId"></param>
        /// <returns></returns>
        List<int> GetUserIdInSupplyChainMPSA(int SupplyChainMPSAId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userInSupplyChainMPSA"></param>
        /// <returns></returns>
        Task CreateUserInSupplyChainMPSAAsync(UserInSupplyChainMPSA userInSupplyChainMPSA);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userInSupplyChainMPSA"></param>
        /// <returns></returns>
        Task DeleteUserInSupplyChainMPSAAsync(UserInSupplyChainMPSA userInSupplyChainMPSA);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SupplyChainMPSAId"></param>
        /// <returns></returns>
        Task DeletaAllUserInSupplyChainMPSA(int SupplyChainMPSAId);



    }
}
