using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Configuration;
using System.Threading.Tasks;
using Entities.Domain;
using Entities.Domain.Users;
using RepositoryPattern.Repositories;
using Utils;
using Utils.Caching;

namespace Service.Lines
{
    public class LineService : BaseService<Line>, ILineService
    {
        protected override string PatternKey
        {
            get { return "PG.line."; }
        }

        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : line code
        /// </remarks>
        private const string LINE_BY_LINECODE_KEY = "PG.line.bylinecode-{0}";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : line code
        /// {1} : departmentId
        /// </remarks>
        private const string LINE_BY_LINECODEANDDEPARTMENTID_KEY = "PG.line.bylinecodeanddepartmentid-{0}-{1}";


        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user identify
        /// {1} : line type
        /// {2} : pageindex
        /// {3} : pagesize
        /// </remarks>
        private const string LINE_SEARCH_KEY = "PG.line.search-{0}-{1}-{2}-{3}";

        #endregion

        private readonly ICacheManager _cacheManager;
        private readonly IRepositoryAsync<Line> _lineRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;

        public LineService(IRepositoryAsync<Line> lineRepositoryAsync,
            ICacheManager cacheManager,
            IRepositoryAsync<User> userRepositoryAsync)
            : base(lineRepositoryAsync, cacheManager)
        {
            _lineRepositoryAsync = lineRepositoryAsync;
            _cacheManager = cacheManager;
            _userRepositoryAsync = userRepositoryAsync;
        }

        public Task<Line> GetLineByLineCode(string lineCode)
        {
            var key = string.Format(LINE_BY_LINECODE_KEY, lineCode);
            return _cacheManager.Get(key, () => _lineRepositoryAsync.Table.FirstOrDefaultAsync(l => l.LineCode == lineCode && l.Active));
        }

        public Task<Line> GetLineByLineCodeAndDepartmentId(string lineCode, int departmentId)
        {
            var key = string.Format(LINE_BY_LINECODEANDDEPARTMENTID_KEY, lineCode, departmentId);
            return _cacheManager.Get(key, () => _lineRepositoryAsync.Table.FirstOrDefaultAsync(l => l.LineCode == lineCode && l.DepartmentId == departmentId));
        }

        public Task<IPagedList<Line>> SearchLines(int userId = 0, int? departmentId = null, bool? active = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = string.Format(LINE_SEARCH_KEY, userId, departmentId.HasValue ? departmentId : 0, pageIndex, pageSize);

            return _cacheManager.Get(key, () =>
            {
                var query = _lineRepositoryAsync.Table.AsQueryable();

                if (departmentId != null)
                    query = query.Where(l => l.DepartmentId == departmentId);

                if (userId != 0)
                    query = query.Where(l => l.Users.Any(u => u.Id == userId));

                if (active != null)
                    query = query.Where(l => l.Active == active);
                return Task.FromResult(new PagedList<Line>(query.OrderBy(l => l.Index), pageIndex, pageSize) as IPagedList<Line>);
            });
        }

        public Task UpdateLineOwner(Line line, IEnumerable<string> usernames)
        {
            var deletionUsers = line.Users.Where(u => !usernames.Contains(u.Username)).ToList();
            foreach (var user in deletionUsers)
            {
                line.Users.Remove(user);
            }
            foreach (var username in usernames)
            {
                if (line.Users.FirstOrDefault(u => u.Username == username) == null)
                {
                    var user = _userRepositoryAsync.Table.FirstOrDefaultAsync(u => u.Username == username);
                    line.Users.Add(user.Result);
                }
            }
            return UpdateAsync(line);
        }
        /*
        public Task<List<Line>> GetAllLines()
        {
            var key = string.Format(LINE_ALL_KEY);
            return _cacheManager.Get(key, () => _lineRepositoryAsync.Table.Where(p => p.LineCode != "DeedmacOperation").ToListAsync());

        }
        //public Task<List<UserLine>> GetUserLineOfLine(int lineId)
        //{
        //    if (lineId <= 0)
        //        return null;
        //    var key = string.Format(USER_LINE_OF_LINE_BY_ID_KEY, lineId);
        //    return _cacheManager.Get(key, () => _userLineRepositoryAsync.Table.Where(p => p.LineId == lineId).ToListAsync());
        //}

        public List<string> GetOwnerOfLine(int LineId)
        {
            if (LineId <= 0)
                return null;
            var key = string.Format(USER_LINE_NAME_OF_LINE_BY_ID_KEY, LineId);
            return _cacheManager.Get(key, () =>
            {
                var result = new List<string>();
                //Khang comment var listUserLine = _userLineRepositoryAsync.Table.Where(p => p.LineId == LineId).ToList();
                //foreach (var userLine in listUserLine)
                //{
                //    var user = _userRepositoryAsync.Table.FirstOrDefault();
                //    //Khang comment var user = _userRepositoryAsync.Table.FirstOrDefault(p => p.Id == userLine.UserId);
                //    if (user != null)
                //    {
                //        result.Add(user.Username);
                //    }
                //}
                return result;
            });
        }

        //public Task CreateUserLineAsync(UserLine userLine)
        //{
        //    _cacheManager.RemoveByPattern(LINE_PATTERN_KEY);

        //    return _userLineRepositoryAsync.InsertAsync(userLine);
        //}

        //public Task DeleteUserLineAsync(UserLine userLine)
        //{
        //    _cacheManager.RemoveByPattern(LINE_PATTERN_KEY);

        //    return _userLineRepositoryAsync.DeleteAsync(userLine);
        //}

        public Task DeletaAllUserOfLine(int lineId)
        {
            _cacheManager.RemoveByPattern(LINE_PATTERN_KEY);

            //Khang comment var listLine = _userLineRepositoryAsync.Table.Where(p => p.LineId == lineId);
            //return _userLineRepositoryAsync.DeleteAsync(listLine);
            return null;
        }

        public Task<List<Line>> GetLineOfEmployee(string employeeId, LineType type)
        {
            var key = string.Format(LINE_BYUSER_AND_TYPEKEY, employeeId, type.ToString());
            return _cacheManager.Get(key, () =>
            {
                if (String.IsNullOrEmpty(employeeId))
                {
                    return _lineRepositoryAsync.Table.Where(p => p.LineTypeId == (int)type && p.LineCode != "DeedmacOperation").OrderBy(p => p.Index).ToListAsync();
                }
                else
                {
                    //Khang comment var allLineUser = _userLineRepositoryAsync.Table.Where(p => p.UserId == employeeId).ToListAsync();
                    var result = new List<Line>();
                    //Khang comment foreach (var line in allLineUser.Result)
                    //{
                    //    var lineSearch = _lineRepositoryAsync.Table.FirstOrDefault(p => p.Id == line.LineId && p.MeasureType == type && p.LineCode != "DeedmacOperation");
                    //    if (lineSearch != null) result.Add(lineSearch);
                    //}
                    result = new List<Line>(result.OrderBy(p => p.Index));
                    return Task.FromResult<List<Line>>(result);
                }
            });


            ////list all line by admin
            //if (String.IsNullOrEmpty(employeeId))
            //{
            //    return _lineRepositoryAsync.Table.Where(p => p.MeasureType == type).ToListAsync();
            //}
            //else
            //{
            //    var allLineUser = _userLineRepositoryAsync.Table.Where(p => p.UserId == employeeId);
            //    var result = new List<Line>();
            //    foreach (var line in allLineUser)
            //    {
            //        var lineSearch = _lineRepositoryAsync.Table.FirstOrDefault(p => p.Id == line.LineId && p.MeasureType == type);
            //        if(lineSearch!=null) result.Add(lineSearch);
            //    }
            //    return Task.FromResult<List<Line>> (result);
            //}
        }

        public Task<List<Line>> GetLineOfEmployeeDeedmacOperation(int employeeId, LineType type)
        {
                if (employeeId == 0)
                {
                    return _lineRepositoryAsync.Table.Where(p => p.LineTypeId == (int)type || p.LineCode == "DeedmacOperation").OrderBy(p => p.Index).ToListAsync();
                }
                else
                {
                    var user = _userService.GetUserByIdAsync(employeeId);

                    var lines = user.Result.Lines.Where(l => l.LineTypeId == (int) type || l.LineCode == "DeedmacOperation").ToList();

                    var lineDeedmacOperation =  _lineRepositoryAsync.Table.Where(p=> p.LineCode == "DeedmacOperation").OrderBy(p => p.Index).ToListAsync();
                    foreach (var line in lineDeedmacOperation.Result)
                    {
                        var lineSearch = _lineRepositoryAsync.Table.FirstOrDefault(p => p.Id == line.Id );
                        if (lineSearch != null) lines.Add(lineSearch);
                    }
                    lines = new List<Line>(lines.OrderBy(p => p.Index));
                    return Task.FromResult<List<Line>>(lines);
                }
        }

        */


    }
}
