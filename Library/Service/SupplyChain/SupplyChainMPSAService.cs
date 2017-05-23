using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Entities.Domain;
using Entities.Domain.Users;
using RepositoryPattern.Repositories;
using Utils.Caching;

namespace Service.SupplyChain
{
    public class SupplyChainMPSAService : ISupplyChainMPSAService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : id
        /// </remarks>
        private const string SupplyChainMPSA_BY_ID_KEY = "PG.SupplyChainMPSA.byid-{0}";

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string SupplyChainMPSA_ALL_KEY = "PG.SupplyChainMPSA.all";

        // <summary>
        /// Key for caching
        /// </summary>
        private const string SupplyChainMPSA_BY_DATE_KEY = "PG.SupplyChainMPSA.byline-{0}";

        // <summary>
        /// Key for caching
        /// </summary>
        private const string SupplyChainMPSA_PATTERN_KEY = "PG.SupplyChainMPSA.";

        /// <summary>
        /// 
        /// </summary>
        private const string USER_OF_SupplyChainMPSA_BY_ID_KEY = "PG.SupplyChainMPSA.user.byid-{0}";

        /// <summary>
        /// 
        /// </summary>
        private const string USER_NAME_OF_SupplyChainMPSA_BY_ID_KEY = "PG.SupplyChainMPSA.user.name.byid-{0}";

        /// <summary>
        /// 
        /// </summary>
        private const string USER_ID_OF_SupplyChainMPSA_BY_ID_KEY = "PG.SupplyChainMPSA.user.id.byid-{0}";


        #endregion

        private readonly ICacheManager _cacheManager;
        private readonly IRepositoryAsync<SupplyChainMPSA> _supplyChainMPSARepositoryAsync;
        private readonly IRepositoryAsync<UserInSupplyChainMPSA> _userSupplyChainMPSARepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;

        public SupplyChainMPSAService(IRepositoryAsync<SupplyChainMPSA> SupplyChainMPSARepositoryAsync, 
            ICacheManager cacheManager, 
            IRepositoryAsync<UserInSupplyChainMPSA> userSupplyChainMPSARepositoryAsync, 
            IRepositoryAsync<User> userRepositoryAsync)
        {
            _supplyChainMPSARepositoryAsync = SupplyChainMPSARepositoryAsync;
            _cacheManager = cacheManager;
            _userSupplyChainMPSARepositoryAsync = userSupplyChainMPSARepositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
        }

        public Task<SupplyChainMPSA> GetSupplyChainMPSAById(int id)
        {
            if (id <= 0)
                return null;
            var key = string.Format(SupplyChainMPSA_BY_ID_KEY, id);
            return _cacheManager.Get(key, () => _supplyChainMPSARepositoryAsync.GetByIdAsync(id));
        }

        public Task<List<SupplyChainMPSA>> GetAllSupplyChainMPSAs()
        {
            var key = string.Format(SupplyChainMPSA_ALL_KEY);
            return _cacheManager.Get(key, () => _supplyChainMPSARepositoryAsync.Table.ToListAsync());
        }
        public Task<List<SupplyChainMPSA>> GetSupplyChainMPSAByDate(DateTime createdDate)
        {
            var startDate = new DateTime(createdDate.Year, createdDate.Month, createdDate.Day);
            var endDate = startDate.AddDays(1);
            var key = string.Format(SupplyChainMPSA_BY_DATE_KEY, createdDate.ToShortDateString());
            return _cacheManager.Get(key, () => _supplyChainMPSARepositoryAsync.Table
                .Where(p => p.CreatedDate >= startDate && p.CreatedDate < endDate).ToListAsync());
        }

        public SupplyChainMPSA GetSupplyChainMPSAMeasureCodeAndDate(string measureCode, DateTime createdDate)
        {
            var startDay = new DateTime(createdDate.Year, createdDate.Month, createdDate.Day);
            var endDay = startDay.AddDays(1);
            var result = _supplyChainMPSARepositoryAsync.Table.FirstOrDefault(p => p.CreatedDate < endDay && p.CreatedDate >= startDay && measureCode.Contains(p.MeasureCode.ToString()));
            
            return result;
        }


        public Task CreateAsync(SupplyChainMPSA supplyChainMPSA)
        {
            _cacheManager.RemoveByPattern(SupplyChainMPSA_PATTERN_KEY);

            return _supplyChainMPSARepositoryAsync.InsertAsync(supplyChainMPSA);
        }

        public Task UpdateAsync(SupplyChainMPSA supplyChainMPSA)
        {
            _cacheManager.RemoveByPattern(SupplyChainMPSA_PATTERN_KEY);
            return _supplyChainMPSARepositoryAsync.UpdateAsync(supplyChainMPSA);
        }

        public Task DeleteAsync(SupplyChainMPSA supplyChainMPSA)
        {
            _cacheManager.RemoveByPattern(SupplyChainMPSA_PATTERN_KEY);

            return _supplyChainMPSARepositoryAsync.DeleteAsync(supplyChainMPSA);
        }

        public Task<List<UserInSupplyChainMPSA>> GetUserInSupplyChainMPSA(int supplyChainMPSAId)
        {
            if (supplyChainMPSAId <= 0)
                return null;
            var key = string.Format(USER_OF_SupplyChainMPSA_BY_ID_KEY, supplyChainMPSAId);
            return _cacheManager.Get(key, () => _userSupplyChainMPSARepositoryAsync.Table.Where(p => p.SupplyChainMPSAId == supplyChainMPSAId).ToListAsync());
        }

        public List<string> GetUserNameInSupplyChainMPSA(int supplyChainMPSAId)
        {

            if (supplyChainMPSAId <= 0)
                return null;
            var key = string.Format(USER_NAME_OF_SupplyChainMPSA_BY_ID_KEY, supplyChainMPSAId);
            return _cacheManager.Get(key, () =>
            {
                var result = new List<string>();
                var listUserLine = _userSupplyChainMPSARepositoryAsync.Table.Where(p => p.SupplyChainMPSAId == supplyChainMPSAId).ToList();
                foreach (var userLine in listUserLine)
                {
                    var user = _userRepositoryAsync.Table.FirstOrDefault(p => p.Id == userLine.UserId);
                    if (user != null)
                    {
                        result.Add(user.Username);
                    }
                }
                return result;
            });
        }

        public List<int> GetUserIdInSupplyChainMPSA(int SupplyChainMPSAId)
        {
            if (SupplyChainMPSAId <= 0)
                return null;
            var key = string.Format(USER_ID_OF_SupplyChainMPSA_BY_ID_KEY, SupplyChainMPSAId);
            return _cacheManager.Get(key, () =>
            {

                return _userSupplyChainMPSARepositoryAsync.Table.Where(p => p.SupplyChainMPSAId == SupplyChainMPSAId).Select(p => p.UserId).ToList();

            });
        }

        public Task CreateUserInSupplyChainMPSAAsync(UserInSupplyChainMPSA userInSupplyChainMPSA)
        {
            _cacheManager.RemoveByPattern(SupplyChainMPSA_PATTERN_KEY);

            return _userSupplyChainMPSARepositoryAsync.InsertAsync(userInSupplyChainMPSA);
        }

        public Task DeleteUserInSupplyChainMPSAAsync(UserInSupplyChainMPSA userInSupplyChainMPSA)
        {
            _cacheManager.RemoveByPattern(SupplyChainMPSA_PATTERN_KEY);

            return _userSupplyChainMPSARepositoryAsync.DeleteAsync(userInSupplyChainMPSA);
        }

        public Task DeletaAllUserInSupplyChainMPSA(int SupplyChainMPSAId)
        {
            _cacheManager.RemoveByPattern(SupplyChainMPSA_PATTERN_KEY);

            var listLine = _userSupplyChainMPSARepositoryAsync.Table.Where(p => p.SupplyChainMPSAId == SupplyChainMPSAId);

            return _userSupplyChainMPSARepositoryAsync.DeleteAsync(listLine);
        }
    }
}
