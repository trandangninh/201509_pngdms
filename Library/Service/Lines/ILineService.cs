using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Domain;
using Utils;

namespace Service.Lines
{
    public interface ILineService : IBaseService<Line>
    {
        /// <summary>
        /// Get line by line code
        /// </summary>
        /// <param name="lineCode">line code</param>
        /// <returns>Line</returns>
        Task<Line> GetLineByLineCode(string lineCode);

        /// <summary>
        /// Get line by line code and departmentId
        /// </summary>
        /// <param name="lineCode, departmentId">line code, departmentId</param>
        /// <returns>Line</returns>
        Task<Line> GetLineByLineCodeAndDepartmentId(string lineCode, int departmentId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="departmentId"></param>
        /// <param name="includeDeedmacOperation"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<IPagedList<Line>> SearchLines(int userId = 0, int? departmentId = null, bool? active = null, int pageIndex = 0, int pageSize = int.MaxValue);
        //Task<IPagedList<Line>> SearchLines(int userId = 0, int? departmentId = null, bool includeDeedmacOperation = true, int pageIndex = 0, int pageSize = int.MaxValue);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="usernames"></param>
        /// <returns></returns>
        Task UpdateLineOwner(Line line, IEnumerable<string> usernames);

        #region old code

        /// <summary>
        /// Get all lines
        /// </summary>
        /// <returns>List of lines</returns>
        //Task<List<Line>> GetAllLines();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineId"></param>
        /// <returns></returns>
        //Task<List<UserLine>> GetUserLineOfLine(int lineId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineId"></param>
        /// <returns></returns>
        //List<string> GetOwnerOfLine(int lineId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userLine"></param>
        /// <returns></returns>
        //Task CreateUserLineAsync(UserLine userLine);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userLine"></param>
        /// <returns></returns>
        //Task DeleteUserLineAsync(UserLine userLine);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineId"></param>
        /// <returns></returns>
        //Task DeletaAllUserOfLine(int lineId);

        /// <summary>
        /// Get lines of user
        /// </summary>
        /// <param name="employeeId">if employeeId is null means that we get all line</param>
        /// <param name="type"></param>
        /// <returns></returns>
        //Task<List<Line>> GetLineOfEmployee(string employeeId,LineType type);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        //Task<List<Line>> GetLineOfEmployeeDeedmacOperation(int employeeId, LineType type);

        //

        #endregion
    }
}
