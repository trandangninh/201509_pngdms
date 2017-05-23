using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Domain;

namespace Service.SupplyChain
{
    public interface ISupplyChainFPQService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<SupplyChainFPQ> GetSupplyChainFPQById(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<SupplyChainFPQ>> GetAllSupplyChainFPQs();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineId"></param>
        /// <param name="createdDate"></param>
        /// <returns></returns>
        Task<List<SupplyChainFPQ>> GetSupplyChainFPQByDate(DateTime createdDate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measureCode"></param>
        /// <param name="createdDate"></param>
        /// <returns></returns>
        SupplyChainFPQ GetSupplyChainFPQMeasureCodeAndDate(string measureCode, DateTime createdDate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SupplyChainFPQ"></param>
        /// <returns></returns>
        Task CreateAsync(SupplyChainFPQ SupplyChainFPQ);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SupplyChainFPQ"></param>
        /// <returns></returns>
        Task UpdateAsync(SupplyChainFPQ SupplyChainFPQ);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SupplyChainFPQ"></param>
        /// <returns></returns>
        Task DeleteAsync(SupplyChainFPQ SupplyChainFPQ);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="SupplyChainFPQId"></param>
        /// <returns></returns>
        Task<List<UserInSupplyChainFpq>> GetUserInSupplyChainFPQ(int SupplyChainFPQId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SupplyChainFPQId"></param>
        /// <returns></returns>
        List<string> GetUserNameInSupplyChainFPQ(int SupplyChainFPQId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SupplyChainFPQId"></param>
        /// <returns></returns>
        List<int> GetUserIdInSupplyChainFPQ(int SupplyChainFPQId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userInSupplyChainFPQ"></param>
        /// <returns></returns>
        Task CreateUserInSupplyChainFPQAsync(UserInSupplyChainFpq userInSupplyChainFPQ);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userInSupplyChainFPQ"></param>
        /// <returns></returns>
        Task DeleteUserInSupplyChainFPQAsync(UserInSupplyChainFpq userInSupplyChainFPQ);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SupplyChainFPQId"></param>
        /// <returns></returns>
        Task DeletaAllUserInSupplyChainFPQ(int SupplyChainFPQId);



    }
}
