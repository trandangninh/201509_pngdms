using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Domain;
using Utils;

namespace Service.Departments
{
    public interface IMeasureService : IBaseService<Measure>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dmsLiquidType"></param>
        /// <returns></returns>
        Task<Measure> GetMeasureByCode(string code, int dmsLiquidType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listMeasureCode"></param>
        /// <returns></returns>
        Task<List<Dms>> GetListDmsByMeasureCode(List<string> listMeasureCode);

        /// <summary>
        /// Get measure by measure code and dmsId
        /// </summary>
        /// <param name="measureCode, dmsId">measure code, dmsId</param>
        /// <returns>Measure</returns>
        Task<Measure> GetMeasureByMeasureCodeAndDmsId(string measureCode, int dmsId);

        Task UpdateMeasureOwner(Measure measure, IEnumerable<string> usernames);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listMeasureCode"></param>
        /// <returns></returns>
        Task<IPagedList<Measure>> GetAllMeasureByDmsId(int? dmsId, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        List<Measure> GetAllMeasureByDepartmentId(int departmentId);
    }
}
