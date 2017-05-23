using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Entities.Domain;

using RepositoryPattern.Repositories;
using Service.Interface;
using Utils.Caching;

namespace Service.Common
{
    public class TrackingService : ITrackingService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : id
        /// </remarks>
        private const string TRACKING_BY_ID_KEY = "PG.tracking.byid-{0}";

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string TRACKING_ALL_KEY = "PG.tracking.all";

        // <summary>
        /// Key for caching
        /// </summary>
        private const string TRACKING_BY_LINE_KEY = "PG.tracking.byline-{0}-{1}";

        // <summary>
        /// Key for caching
        /// </summary>
        private const string TRACKING_BY_TWODATE_LINE_KEY = "PG.tracking.byline-{0}-{1}-{2}";

        // <summary>
        /// Key for caching
        /// </summary>
        private const string TRACKING_PATTERN_KEY = "PG.tracking.";



        #endregion

        private readonly ICacheManager _cacheManager;
        private readonly IRepositoryAsync<Tracking> _trackingRepositoryAsync;
        public TrackingService(IRepositoryAsync<Tracking> trackingRepositoryAsync, 
            ICacheManager cacheManager)
        {
            _trackingRepositoryAsync = trackingRepositoryAsync;
            _cacheManager = cacheManager;
        }

        public Task<Tracking> GetTrackingById(int id)
        {
            if (id <= 0)
                return null;
            var key = string.Format(TRACKING_BY_ID_KEY, id);
            return _cacheManager.Get(key, () => _trackingRepositoryAsync.GetByIdAsync(id));
        }

        public Task<List<Tracking>> GetAllTrackings()
        {
            var key = string.Format(TRACKING_ALL_KEY);
            return _cacheManager.Get(key, () => _trackingRepositoryAsync.Table.Include(p => p.CreatedUser).ToListAsync());
        }

        public Task<List<Tracking>> GetTrackingByDateAndLine(string lineCode, DateTime createdDate)
        {
            var startDate = new DateTime(createdDate.Year, createdDate.Month, createdDate.Day);
            var endDate = startDate.AddDays(1);
            //var endDate = new DateTime(createdDate.Year, createdDate.Month, createdDate.Day + 1);
            var key = string.Format(TRACKING_BY_LINE_KEY, lineCode, createdDate.ToShortDateString());
            return _cacheManager.Get(key, () => _trackingRepositoryAsync.Table
                .Where(p => p.CreatedDate >= startDate && p.CreatedDate < endDate && p.LineCode == lineCode).ToListAsync());
        }

        public Task<List<Tracking>> GetTrackingByTwoDateAndLine(string lineCode, DateTime fromDate, DateTime toDate)
        {
           
            //var endDate = new DateTime(createdDate.Year, createdDate.Month, createdDate.Day + 1)
            var key = string.Format(TRACKING_BY_TWODATE_LINE_KEY, lineCode, fromDate, toDate);
            return _cacheManager.Get(key, () => _trackingRepositoryAsync.Table
                .Where(p => p.CreatedDate >= fromDate && p.CreatedDate <= toDate && p.LineCode == lineCode).ToListAsync());
           
        }

        public Task<List<Tracking>> GetTrackingOfLine(string lineId)
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(Tracking tracking)
        {
            _cacheManager.RemoveByPattern(TRACKING_PATTERN_KEY);

            return _trackingRepositoryAsync.InsertAsync(tracking);
        }

        public Task UpdateAsync(Tracking tracking)
        {
            _cacheManager.RemoveByPattern(TRACKING_PATTERN_KEY);
            return _trackingRepositoryAsync.UpdateAsync(tracking);
        }

        public Task DeleteAsync(Tracking tracking)
        {
            _cacheManager.RemoveByPattern(TRACKING_PATTERN_KEY);

            return _trackingRepositoryAsync.DeleteAsync(tracking);
        }

      
    }
}
