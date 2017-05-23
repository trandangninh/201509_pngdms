using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Entities.Domain;
using Entities.Domain.Users;
using RepositoryPattern.Repositories;
using Service.Interface;
using Service.Users;
using Utils;
using Utils.Caching;

namespace Service.Departments
{
    public class IssueService : BaseService<Issue>, IIssueService
    {
        private readonly IRepositoryAsync<Issue> _issueRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IUserService _userService;
        private readonly ICacheManager _cacheManager;
        private readonly IWorkContext _workContext;

        protected override string PatternKey
        {
            get { return "Pg.Issue"; }
        }

        public IssueService(IRepositoryAsync<Issue> issueRepositoryAsync, 
            ICacheManager cacheManager,
            IWorkContext workContext,
            IRepositoryAsync<User> userRepositoryAsync, 
            IUserService userService):base(issueRepositoryAsync, cacheManager)
        {
            _issueRepositoryAsync = issueRepositoryAsync;
            _cacheManager = cacheManager;
            _userRepositoryAsync = userRepositoryAsync;
            _userService = userService;
            _workContext = workContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">asigned user</param>
        /// <param name="searchKeyWord"></param>
        /// <param name="status"></param>
        /// <param name="departmentId"></param>
        /// <param name="createdDate"></param>
        /// <param name="oldDate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Task<IPagedList<Issue>> SearchIssues(bool includeAllUserInDepartment = false, List<int> listUserId = null, bool includeUserCreated = false, string searchKeyWord = null, List<int> status = null, int departmentId = 0,
            DateTime? createdDate = null, bool oldDate = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _issueRepositoryAsync.Table.AsQueryable();
            if (includeAllUserInDepartment && _workContext.CurrentUser!=null)
            {
                var udIds = _workContext.CurrentUser.Departments.Select(d => d.Id).ToList();
                query = query.Where(i => udIds.Any(id=>id == i.DepartmentId));
            } 
            if (listUserId != null)
            {
                var user = _userService.GetByIdAsync(listUserId.FirstOrDefault()).Result;
                query = query.Where(i => listUserId.Contains(i.UserOwnerId) || (listUserId.Contains(i.UserId) && includeUserCreated));//.Where(i => (departmentId == 0 || i.DepartmentId == user.DepartmentId));
            }                

            if (!String.IsNullOrEmpty(searchKeyWord))
                query = query.Where(i => i.ActionPlan.Contains(searchKeyWord) || i.Content.Contains(searchKeyWord));
            if (status != null)
                query = query.Where(i => status.Contains(i.IssueStatusId));
            if (departmentId != 0)
                query = query.Where(i => i.DepartmentId == departmentId);
            if (createdDate != null && createdDate != new DateTime())
            {
                var startDate = new DateTime(createdDate.Value.Year, createdDate.Value.Month, createdDate.Value.Day);
                var endDate = startDate.AddDays(1);
                query = query.Where(i => (oldDate || i.WhenDue >= startDate) && i.WhenDue < endDate);
            }
            return Task.FromResult(new PagedList<Issue>(query.OrderByDescending(i => i.WhenDue), pageIndex, pageSize) as IPagedList<Issue>);
        }
        public Task<Issue> GetIssueById(int id)
        {
            return _issueRepositoryAsync.GetByIdAsync(id);
        }
        
        public Task<List<Issue>> GetAllIssues()
        {
            return _issueRepositoryAsync.Table.OrderByDescending(p => p.WhenDue).Include(p => p.User).ToListAsync();
        }

        public List<Issue> GetAllIssuesNotAsync()
        {
            return _issueRepositoryAsync.Table.OrderByDescending(p => p.WhenDue).Include(p => p.User).ToList();
        }

        public Task<List<Issue>> GetIssueByDateAndTypeAndUser(int userId, DateTime createdDate, int departmentId)
        {
            var startDate = new DateTime(createdDate.Year, createdDate.Month, createdDate.Day);
            var endDate = startDate.AddDays(1);

            var query = _issueRepositoryAsync.Table.Where(i => i.DepartmentId == departmentId &&
                    i.CreatedDate >= startDate && i.CreatedDate <= endDate);
            if (userId != 0)
            {
                query = query.Where(i => i.UserId == userId);
            }

            return query.OrderByDescending(p => p.WhenDue).ToListAsync();
        }

        public List<Issue> GetIssueByDate(DateTime createdDate)
        {
            var searchDate = new DateTime(createdDate.Year, createdDate.Month, createdDate.Day);
            var endDate = searchDate.AddDays(1);
            return _issueRepositoryAsync.Table.Where(p => p.WhenDue >= searchDate && p.WhenDue < endDate).ToList();
        }

        public List<Issue> GetIssueOld(DateTime whenDue)
        {
            var searchDate = new DateTime(whenDue.Year, whenDue.Month, whenDue.Day);
            var endDate = searchDate.AddDays(1);
            return _issueRepositoryAsync.Table.Where(p => p.WhenDue < endDate).ToList();
     
        }

        public List<Issue> GetIssueOld(DateTime whenDue, int type)
        {
            var searchDate = new DateTime(whenDue.Year, whenDue.Month, whenDue.Day);
            var endDate = searchDate.AddDays(1);
            return _issueRepositoryAsync.Table.Where(p => p.WhenDue < endDate && p.IssueStatusId == type).ToList();
        }

        //public Task<List<Issue>> GetIssuesOfUser(int userId)
        //{
        //    if (userId == 0) return Task.FromResult<List<Issue>>(null);
        //    var userIssues =
        //           _userIssueRepositoryAsync.Table.Where(p => p.UserId == userId).Select(p => p.IssueId);

        //    var issues =
        //        _issueRepositoryAsync.Table.Include(p => p.User)
        //            .Where(
        //                p =>userIssues.Contains(p.Id));
        //    return issues.OrderByDescending(p => p.WhenDue).ToListAsync();
        //}

        public Task<List<Issue>> GetIssuesOfListUser(List<int> lstUser)
        {
            if (lstUser == null) return Task.FromResult<List<Issue>>(null);
            var issueses = _issueRepositoryAsync.Table.Where(p => lstUser.Any(lu => lu == p.UserId));

            return issueses.OrderByDescending(p => p.WhenDue).ToListAsync();
        }

        //public List<Issue> GetIssuesOfUserNotAsync(int userId)
        //{
        //    if (lstUser == null) return Task.FromResult<List<Issue>>(null);
        //    var issueses = _issueRepositoryAsync.Table.Where(p => lstUser.Any(lu => lu == p.UserId));

        //    return issueses.OrderByDescending(p => p.WhenDue).ToListAsync();
        //}

        //public Task<List<Issue>> GetOpenIssueOfUser(int userId)
        //{
        //    if (userId == 0) return Task.FromResult<List<Issue>>(null);
        //    var userIssues =
        //           _userIssueRepositoryAsync.Table.Where(p => p.UserId == userId).Select(p => p.IssueId);

        //    var issues =
        //        _issueRepositoryAsync.Table.Include(p => p.User)
        //            .Where(
        //                p => userIssues.Contains(p.Id)&&p.Status==IssueStatus.Open);
        //    return issues.OrderByDescending(p => p.WhenDue).ToListAsync();
        //}

        
        public void UpdateNotAsync(Issue issue)
        {
            if (issue == null)
                throw new ArgumentNullException("issue");

            _issueRepositoryAsync.Update(issue);
        }


       

        public List<string> GetListUserAssignedOfIssue(int issueId)
        {
            if (issueId <= 0)
                return null;
            var result = new List<string>();
            //Khang comment var listUserDms = _userIssueRepositoryAsync.Table.Where(p => p.IssueId == issueId).ToList();
            //foreach (var userDms in listUserDms)
            //{
            //    var user = _userRepositoryAsync.Table.FirstOrDefault();
            //    //Khang comment var user = _userRepositoryAsync.Table.FirstOrDefault(p => p.Id == userDms.UserId);
            //    if (user != null)
            //    {
            //        result.Add(user.Username);
            //    }
            //}
            return result;
        }

        public string GetAssignUserId(int issueId)
        {
            if (issueId <= 0)
                return "";
           
            //Khang comment var listUserDms = _userIssueRepositoryAsync.Table.Where(p => p.IssueId == issueId).ToList();
            //var firstUserIssue = listUserDms.FirstOrDefault();
            //if (firstUserIssue == null) return "";
            var user = _userRepositoryAsync.Table.FirstOrDefault();
            //Khang comment var user = _userRepositoryAsync.Table.FirstOrDefault(p => p.Id == firstUserIssue.UserId)
            return user.Id.ToString(); //<== check here
        }


        public System.Data.DataTable ReadExcellToDataTable(string url)
        {
            var dt = new System.Data.DataTable();
            return dt;
        }


        public Task DeleteIssuesAsync(List<int> listId)
        {
            if (listId == null)
                throw new ArgumentNullException("listId");
            var deletedIssues = _issueRepositoryAsync.Table.Where(i => listId.Contains(i.Id));
            _cacheManager.RemoveByPattern(PatternKey);
            return _issueRepositoryAsync.DeleteAsync(deletedIssues);
        }
    }
}
