using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Entities.Domain;
using Entities.Domain.Users;

using RepositoryPattern.Repositories;
using Utils;
using Utils.Caching;

namespace Service.Departments
{
    public class DmsService : BaseService<Dms>, IDmsService
    {
        #region constant for cache
        protected override string PatternKey
        {
            get { return "PG.dms."; }
        }

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : departmentId
        /// </remarks>
        private const string DMS_BY_DEPARTMENTID_KEY = "PG.dms.bydepartmentid-{0}";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : departmentId
        /// </remarks>
        private const string DMSLIST_BY_DEPARTMENTID_KEY = "PG.dms.listdmsbydepartmentid-{0}";

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string DMS_ALL_KEY = "PG.dms.all-{0}-{1}";

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string DMS_ALL_KEY_NOT_ASYNC = "PG.dms.all.notasync";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string DMS_PATTERN_KEY = "PG.dms.";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : id
        /// </remarks>
        private const string USER_DMS_OF_DMS_BY_ID_KEY = "PG.dms.userdms.byid-{0}";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : id
        /// </remarks>
        private const string USER_DMS_NAME_OF_DMS_BY_ID_KEY = "PG.dms.userdms.name.byid-{0}";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : typeid
        /// {1} : departmentid
        /// </remarks>
        private const string DMS_BY_TYPE_DEPARTMENT_ID_KEY = "PG.dms.bytypedepartmentid-{0}-{1}";

        #endregion

        private readonly ICacheManager _cacheManager;
        private readonly IRepositoryAsync<Dms> _dmsRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync; 
        

        public DmsService(IRepositoryAsync<Dms> dmsRepositoryAsync, 
            ICacheManager cacheManager,
            IRepositoryAsync<User> userRepositoryAsync) : base (dmsRepositoryAsync, cacheManager)

        {
            _dmsRepositoryAsync = dmsRepositoryAsync;
            _cacheManager = cacheManager;
            _userRepositoryAsync = userRepositoryAsync;
        }

        public Task<Dms> GetDmsByDmsCode(string dmsCode)
        {
            if (String.IsNullOrEmpty(dmsCode))
                return null;

            return _dmsRepositoryAsync.Table.FirstOrDefaultAsync(d => d.DmsCode == dmsCode && d.Active);
        }

        public Task<Dms> GetDmsByDmsCodeAndDepartmentId(string dmsCode, int departmentId)
        {
            if (String.IsNullOrEmpty(dmsCode) || departmentId < 1)
                return null;

            return _dmsRepositoryAsync.Table.FirstOrDefaultAsync(d => d.DmsCode == dmsCode && d.DepartmentId == departmentId);
        }

        public Task<Dms> GetDmsByTypeAndDepartmentId(DmsType type, int departmentId)
        {
            var key = string.Format(DMS_BY_TYPE_DEPARTMENT_ID_KEY, (int) type, departmentId);

            return _cacheManager.Get(key, () => _dmsRepositoryAsync.Table.FirstOrDefaultAsync(d => d.DmsTypeId == (int) type && d.DepartmentId == departmentId && d.Active));
        }

        public Task<Dms> GetDmsByDepartmentId(int departmentId)
        {
            if (departmentId <= 0)
                return null;

            var key = string.Format(DMS_BY_DEPARTMENTID_KEY, departmentId);
            return _cacheManager.Get(key, () => _dmsRepositoryAsync.Table.FirstOrDefaultAsync(d => d.DepartmentId == departmentId && d.Active));
        }

        //get all dms(both active and not active) by departmentId
        public Task<IPagedList<Dms>> GetDmsByDepartmentId(int departmentId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            if (departmentId <= 0)
                return null;

            var key = string.Format(DMSLIST_BY_DEPARTMENTID_KEY, departmentId);
            return _cacheManager.Get(key, () => Task.FromResult(new PagedList<Dms>(_dmsRepositoryAsync.Table.Where(d => d.DepartmentId == departmentId).OrderBy(d => d.Order), pageIndex, pageSize) as IPagedList<Dms>));
        }



        public IPagedList<Dms> GetAllDmss(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = string.Format(DMS_ALL_KEY,pageIndex,pageSize);
            return _cacheManager.Get(key, () =>new PagedList<Dms>(_dmsRepositoryAsync.Table.OrderBy(d => d.Order), pageIndex, pageSize));
        }

        public List<Dms> GetAllDmssNotAsync()
        {
            var key = string.Format(DMS_ALL_KEY_NOT_ASYNC);
            return _cacheManager.Get(key, () => _dmsRepositoryAsync.Table.ToList());
        }

        public Task InsertDmsAsync(Dms dms)
        {
            if (dms == null)
                throw new ArgumentNullException("dms");

            _cacheManager.RemoveByPattern(DMS_PATTERN_KEY);

            return _dmsRepositoryAsync.InsertAsync(dms);
        }

        public Task UpdateAsync(Dms dms)
        {
            if (dms == null)
                throw new ArgumentNullException("dms");

            _cacheManager.RemoveByPattern(DMS_PATTERN_KEY);

            return _dmsRepositoryAsync.UpdateAsync(dms);
        }

        public Task DeleteAsync(Dms dms)
        {
            if(dms == null)
                throw new ArgumentNullException("dms");

            _cacheManager.RemoveByPattern(DMS_PATTERN_KEY);

            return _dmsRepositoryAsync.DeleteAsync(dms);
        }

        //public Task<List<UserDms>> GetUserDmsOfDms(int dmsId)
        //{
        //    if (dmsId <= 0)
        //        return null;
        //    var key = string.Format(USER_DMS_OF_DMS_BY_ID_KEY, dmsId);
        //    return _cacheManager.Get(key, () => _userDmsRepositoryAsync.Table.Where(p => p.DmsId == dmsId).ToListAsync());
        //}

        public List<string> GetOwnerOfDms(int dmsId)
        {
            if (dmsId <= 0)
                return null;
            var key = string.Format(USER_DMS_NAME_OF_DMS_BY_ID_KEY, dmsId);
            return _cacheManager.Get(key, () =>
            {
                var result = new List<string>();
                //Khang comment var listUserDms = _userDmsRepositoryAsync.Table.Where(p => p.DmsId == dmsId).ToList();
                //foreach (var userDms in listUserDms)
                //{
                //    //Khang comment var user = _userRepositoryAsync.Table.FirstOrDefault(p => p.Id == userDms.UserId);
                //    var user = _userRepositoryAsync.Table.FirstOrDefault();
                //    if (user != null)
                //    {
                //        result.Add(user.Username);
                //    } 
                //}
                return result;
            });
        }

        //public Task CreateUserDmsAsync(UserDms userDms)
        //{
        //    _cacheManager.RemoveByPattern(DMS_PATTERN_KEY);

        //    _userDmsRepositoryAsync.Insert(userDms);
        //    return null;//_unitOfWorkAsync.SaveChangesAsync();
        //}

        //public Task DeleteUserDmsAsync(UserDms userDms)
        //{
        //    _cacheManager.RemoveByPattern(DMS_PATTERN_KEY);

        //    _userDmsRepositoryAsync.Delete(userDms);
        //    return null;//_unitOfWorkAsync.SaveChangesAsync();
        //}

        public Task DeletaAllUserOfDms(int dmsId)
        {
            _cacheManager.RemoveByPattern(DMS_PATTERN_KEY);

            //Khang comment var listDms = _userDmsRepositoryAsync.Table.Where(p => p.DmsId == dmsId);
            //foreach (var userDmse in listDms)
            //{
            //    _userDmsRepositoryAsync.Delete(userDmse);
            //}
            return null;//_unitOfWorkAsync.SaveChangesAsync();
        }

        public Task UpdateDmsOwner(Dms dms, IEnumerable<string> usernames)
        {
            if (dms == null)
                throw new ArgumentNullException("dms");

            var deletionUsers = dms.Users.Where(u => !usernames.Contains(u.Username)).ToList();
            foreach (var user in deletionUsers)
            {
                dms.Users.Remove(user);
            }
            foreach (var username in usernames)
            {
                if (dms.Users.FirstOrDefault(u => u.Username == username) == null)
                {
                    var user = _userRepositoryAsync.Table.FirstOrDefaultAsync(u => u.Username == username);
                    dms.Users.Add(user.Result);
                }
            }
            return UpdateAsync(dms);
        }
    }
}
