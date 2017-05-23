using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;
using Utils;

namespace Service.Interface
{
    public interface IIssueService : IBaseService<Issue>
    {
        Task<IPagedList<Issue>> SearchIssues(bool includeAllUserInDepartment = false, List<int> userId = null,bool includeUserCreated = false, string searchKeyword = null, List<int> statusId = null, int departmentId = 0, DateTime? createdDate = null, bool oldDate = false, int pageIndex = 0, int pageSize = int.MaxValue);















        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Issue> GetIssueById(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<Issue>> GetAllIssues();


        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<Issue> GetAllIssuesNotAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="createdDate"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        //Task<List<Issue>> GetIssueByDateAndTypeAndUser(int userId,DateTime createdDate,IssueType type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createdDate"></param>
        /// <returns></returns>
        //List<Issue> GetIssueByDate( DateTime createdDate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whenDue"></param>
        /// <returns></returns>
        //List<Issue> GetIssueOld(DateTime whenDue);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whenDue"></param>
        /// <returns></returns>
        //List<Issue> GetIssueOld(DateTime whenDue, int type);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        //Task<List<Issue>> GetIssuesOfUser(int userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lstUser"></param>
        /// <returns></returns>
        Task<List<Issue>> GetIssuesOfListUser(List<int> lstUser);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        //List<Issue> GetIssuesOfUserNotAsync(int userId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        //Task<List<Issue>> GetOpenIssueOfUser(int userId); 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issue"></param>
        void UpdateNotAsync(Issue issue);


        List<string>  GetListUserAssignedOfIssue(int issueId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns></returns>
        string GetAssignUserId(int issueId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userIssue"></param>
        /// <returns></returns>
        //Task CreateUserIssueAsync(UserIssue userIssue);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userIssue"></param>
        /// <returns></returns>
        //Task DeleteUserIssueAsync(UserIssue userIssue);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns></returns>
        //Task DeletaAllUserOfIssue(int issueId);

        System.Data.DataTable ReadExcellToDataTable(string url);

        Task DeleteIssuesAsync(List<int> listId);
    }
}
