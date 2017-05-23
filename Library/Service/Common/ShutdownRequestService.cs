using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;


using RepositoryPattern.Repositories;
using Service.Interface;
using Utils;
using Utils.Caching;

namespace Service.Common
{
    public class ShutdownRequestService : BaseService<ShutdownRequest>, IShutdownRequestService
    {
        private readonly ICacheManager _cacheManager;
        private readonly IRepositoryAsync<ShutdownRequest> _shutdownRequestRepositoryAsync;


        public ShutdownRequestService(IRepositoryAsync<ShutdownRequest> shutdownRequestRepositoryAsync,
            ICacheManager cacheManager)
            : base(shutdownRequestRepositoryAsync, cacheManager)
        {
            _shutdownRequestRepositoryAsync = shutdownRequestRepositoryAsync;
            _cacheManager = cacheManager;
        }

        protected override string PatternKey
        {
            get { return "PG.shutdownrequest"; }
        }

        public IPagedList<ShutdownRequest> GetShutdownRequest(string keyword = "", int userId = 0, int statusId = 0, DateTime? startDate = null,
            DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _shutdownRequestRepositoryAsync.Table.AsQueryable();

            if (!String.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.ShutdownRequestContent.ToLower().Contains(keyword.ToLower()));
            }
            if (userId != 0)
            {
                query = query.Where(p => p.UserCreatedId == userId);
            }
            if (statusId != 0)
            {
                query = query.Where(p => p.ShutdownStatusId == statusId);
            }
            if (startDate != null)
            {
                query = query.Where(p => p.BasePlanDate >= startDate);
            }
            if (endDate != null)
            {
                query = query.Where(p => p.BasePlanDate <= endDate);
            }

            return new PagedList<ShutdownRequest>(query.OrderByDescending(p=>p.UpdatedDate),pageIndex, pageSize);
        }
    }
}
