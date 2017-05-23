using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Domain;
using Utils;

namespace Service.Departments
{
    public interface IDmsService : IBaseService<Dms>
    {
        /// <summary>
        /// Get DMS by DMS code
        /// </summary>
        /// <param name="dmsCode">DMS code</param>
        /// <returns></returns>
        Task<Dms> GetDmsByDmsCode(string dmsCode);

        /// <summary>
        /// Update DMS owner
        /// </summary>
        /// <param name="dms">DMS object</param>
        /// <param name="usernames">List of usernames</param>
        /// <returns></returns>
        //Dms GetDmsByDMSCode(string dmsCode);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        Task<Dms> GetDmsByDepartmentId(int departmentId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        Task<IPagedList<Dms>> GetDmsByDepartmentId(int departmentId, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IPagedList<Dms> GetAllDmss(int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<Dms> GetAllDmssNotAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dms"></param>
        /// <returns></returns>
        Task InsertDmsAsync(Dms dms);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dms"></param>
        /// <returns></returns>
        Task UpdateAsync(Dms dms);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dms"></param>
        /// <returns></returns>
        Task DeleteAsync(Dms dms);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dmsId"></param>
        /// <returns></returns>
        //Task<List<UserDms>> GetUserDmsOfDms(int dmsId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dmsId"></param>
        /// <returns></returns>
        List<string>  GetOwnerOfDms(int dmsId); 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userDms"></param>
        /// <returns></returns>
        //Task CreateUserDmsAsync(UserDms userDms);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userDms"></param>
        /// <returns></returns>
        //Task DeleteUserDmsAsync(UserDms userDms);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dmsId"></param>
        /// <returns></returns>
        Task DeletaAllUserOfDms(int dmsId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dms"></param>
        /// <param name="usernames"></param>
        /// <returns></returns>
        Task UpdateDmsOwner(Dms dms, IEnumerable<string> usernames);

        
        Task<Dms> GetDmsByDmsCodeAndDepartmentId(string dmsCode, int departmentId);
        Task<Dms> GetDmsByTypeAndDepartmentId(DmsType type, int departmentId);
    }
}
