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
    public class SupplyChainFPQService : ISupplyChainFPQService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : id
        /// </remarks>
        private const string SupplyChainFPQ_BY_ID_KEY = "PG.SupplyChainFPQ.byid-{0}";

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string SupplyChainFPQ_ALL_KEY = "PG.SupplyChainFPQ.all";

        // <summary>
        /// Key for caching
        /// </summary>
        private const string SupplyChainFPQ_BY_DATE_KEY = "PG.SupplyChainFPQ.byline-{0}";

        // <summary>
        /// Key for caching
        /// </summary>
        private const string SupplyChainFPQ_PATTERN_KEY = "PG.SupplyChainFPQ.";

        /// <summary>
        /// 
        /// </summary>
        private const string USER_OF_SupplyChainFPQ_BY_ID_KEY = "PG.SupplyChainFPQ.user.byid-{0}";

        /// <summary>
        /// 
        /// </summary>
        private const string USER_NAME_OF_SupplyChainFPQ_BY_ID_KEY = "PG.SupplyChainFPQ.user.name.byid-{0}";

        /// <summary>
        /// 
        /// </summary>
        private const string USER_ID_OF_SupplyChainFPQ_BY_ID_KEY = "PG.SupplyChainFPQ.user.id.byid-{0}";
        

        #endregion

        private readonly ICacheManager _cacheManager;
        private readonly IRepositoryAsync<SupplyChainFPQ> _SupplyChainFPQRepositoryAsync;
        private readonly IRepositoryAsync<UserInSupplyChainFpq> _userSupplyChainFPQRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;

        public SupplyChainFPQService(IRepositoryAsync<SupplyChainFPQ> repository, 
            IRepositoryAsync<SupplyChainFPQ> SupplyChainFPQRepositoryAsync, 
            ICacheManager cacheManager/*, IUnitOfWorkAsync unitOfWorkAsync*/, 
            IRepositoryAsync<UserInSupplyChainFpq> userSupplyChainFpqRepositoryAsync, 
            IRepositoryAsync<User> userRepositoryAsync)
        {
            _SupplyChainFPQRepositoryAsync = SupplyChainFPQRepositoryAsync;
            _cacheManager = cacheManager;
            _userSupplyChainFPQRepositoryAsync = userSupplyChainFpqRepositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
        }

        public Task<SupplyChainFPQ> GetSupplyChainFPQById(int id)
        {
            if (id <= 0)
                return null;
            var key = string.Format(SupplyChainFPQ_BY_ID_KEY, id);
            return _cacheManager.Get(key, () => _SupplyChainFPQRepositoryAsync.GetByIdAsync(id));
        }

        public Task<List<SupplyChainFPQ>> GetAllSupplyChainFPQs()
        {
            var key = string.Format(SupplyChainFPQ_ALL_KEY);
            return _cacheManager.Get(key, () => _SupplyChainFPQRepositoryAsync.Table.ToListAsync());
        }
        public Task<List<SupplyChainFPQ>> GetSupplyChainFPQByDate( DateTime createdDate)
        {
            var startDate = new DateTime(createdDate.Year, createdDate.Month, createdDate.Day);
            var endDate = startDate.AddDays(1);
            var key = string.Format(SupplyChainFPQ_BY_DATE_KEY, createdDate.ToShortDateString());
            return _cacheManager.Get(key, () => _SupplyChainFPQRepositoryAsync.Table
                .Where(p => p.CreatedDate >= startDate && p.CreatedDate < endDate ).ToListAsync());
        }

        public SupplyChainFPQ GetSupplyChainFPQMeasureCodeAndDate(string measureCode, DateTime createdDate)
        {
            var startDay = new DateTime(createdDate.Year, createdDate.Month, createdDate.Day);
            var endDay = startDay.AddDays(1);
            var listResultInDay = _SupplyChainFPQRepositoryAsync.Table.Where(p => p.CreatedDate < endDay && p.CreatedDate >= startDay);
            var result = listResultInDay.FirstOrDefault(p => measureCode.Contains(p.MeasureCode.ToString()));
            return result;
        }


        public Task CreateAsync(SupplyChainFPQ supplyChainFPQ)
        {
            _cacheManager.RemoveByPattern(SupplyChainFPQ_PATTERN_KEY);

            return _SupplyChainFPQRepositoryAsync.InsertAsync(supplyChainFPQ);
        }

        public Task UpdateAsync(SupplyChainFPQ supplyChainFPQ)
        {
            _cacheManager.RemoveByPattern(SupplyChainFPQ_PATTERN_KEY);
            return _SupplyChainFPQRepositoryAsync.UpdateAsync(supplyChainFPQ);
        }

        public Task DeleteAsync(SupplyChainFPQ supplyChainFPQ)
        {
            _cacheManager.RemoveByPattern(SupplyChainFPQ_PATTERN_KEY);

            return _SupplyChainFPQRepositoryAsync.DeleteAsync(supplyChainFPQ);
        }

        public Task<List<UserInSupplyChainFpq>> GetUserInSupplyChainFPQ(int supplyChainFPQId)
        {
            if (supplyChainFPQId <= 0)
                return null;
            var key = string.Format(USER_OF_SupplyChainFPQ_BY_ID_KEY, supplyChainFPQId);
            return _cacheManager.Get(key, () => _userSupplyChainFPQRepositoryAsync.Table.Where(p => p.SupplyChainFpqId == supplyChainFPQId).ToListAsync());
        }

        public List<string> GetUserNameInSupplyChainFPQ(int supplyChainFPQId)
        {

            if (supplyChainFPQId <= 0)
                return null;
            var key = string.Format(USER_NAME_OF_SupplyChainFPQ_BY_ID_KEY, supplyChainFPQId);
            return _cacheManager.Get(key, () =>
            {
                var result = new List<string>();
                var listUserLine = _userSupplyChainFPQRepositoryAsync.Table.Where(p => p.SupplyChainFpqId == supplyChainFPQId).ToList();
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

        public List<int> GetUserIdInSupplyChainFPQ(int SupplyChainFPQId)
        {
            if (SupplyChainFPQId <= 0)
                return null;
            var key = string.Format(USER_ID_OF_SupplyChainFPQ_BY_ID_KEY, SupplyChainFPQId);
            return _cacheManager.Get(key, () =>
            {
                return _userSupplyChainFPQRepositoryAsync.Table.Where(p => p.SupplyChainFpqId == SupplyChainFPQId).Select(p => p.UserId).ToList();

            });
        }

        public Task CreateUserInSupplyChainFPQAsync(UserInSupplyChainFpq userInSupplyChainFPQ)
        {
            _cacheManager.RemoveByPattern(SupplyChainFPQ_PATTERN_KEY);

            return _userSupplyChainFPQRepositoryAsync.InsertAsync(userInSupplyChainFPQ);
        }

        public Task DeleteUserInSupplyChainFPQAsync(UserInSupplyChainFpq userInSupplyChainFPQ)
        {
            _cacheManager.RemoveByPattern(SupplyChainFPQ_PATTERN_KEY);

            return _userSupplyChainFPQRepositoryAsync.DeleteAsync(userInSupplyChainFPQ);
        }

        public Task DeletaAllUserInSupplyChainFPQ(int SupplyChainFPQId)
        {
            _cacheManager.RemoveByPattern(SupplyChainFPQ_PATTERN_KEY);

            var listLine = _userSupplyChainFPQRepositoryAsync.Table.Where(p => p.SupplyChainFpqId == SupplyChainFPQId);

            return _userSupplyChainFPQRepositoryAsync.DeleteAsync(listLine);
        }
    }
}
