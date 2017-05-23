using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;

using Entities.Domain.Users;

namespace Service.Interface
{
    public interface IUserAllowInSupplyChainService
    {
        Task<List<UserAllowInSupplyChain>> GetUserByDMSType(DmsType type);

        bool CheckUserByDMSTypeNotAsync(DmsType type, string userName);


        Task<bool> CheckUserByDMSType(DmsType type, string userName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<List<string>> GetUsernameByDMSType(DmsType type);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Task DeleteAllUserOfType(DmsType type);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task AddUserToDMS(DmsType type, User user);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userAllowInSupplyChain"></param>
        /// <returns></returns>
        Task CreateAsync(UserAllowInSupplyChain userAllowInSupplyChain);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userAllowInSupplyChain"></param>
        /// <returns></returns>
        Task UpdateAsync(UserAllowInSupplyChain userAllowInSupplyChain);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userAllowInSupplyChain"></param>
        /// <returns></returns>
        Task DeleteAsync(UserAllowInSupplyChain userAllowInSupplyChain);


    }
}
