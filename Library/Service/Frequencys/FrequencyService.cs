using Entities.Domain.Frequencys;
using RepositoryPattern.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Caching;
using Utils;

namespace Service.Frequencys
{
    public class FrequencyService : BaseService<Frequency>, IFrequencyService
    {
        /// <summary>
        /// Key for caching
        /// </summary>
        /// </remarks>
        private const string FREQUENCY_LISTPAGED_KEY = "PG.Frequency.ListPaged-{0}-{1}";


        protected override string PatternKey
        {
            get { return "PG.Frequency."; }
        }

        private readonly ICacheManager _cacheManager;
        private readonly IRepositoryAsync<Frequency> _frequencyRepositoryAsync;

        public FrequencyService(ICacheManager cacheManager,
            IRepositoryAsync<Frequency> frequencyRepositoryAsync)
            : base(frequencyRepositoryAsync, cacheManager)
        {
            _frequencyRepositoryAsync = frequencyRepositoryAsync;
            _cacheManager = cacheManager;
        }

        public Task<IPagedList<Frequency>> GetAllFrequencyAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = string.Format(FREQUENCY_LISTPAGED_KEY, pageIndex, pageSize);

            return _cacheManager.Get(key, () =>
            {
                var query = _frequencyRepositoryAsync.Table.AsQueryable();

                //defaulf order by name
                query = query.OrderBy(c => c.Mark);
                return Task.FromResult(new PagedList<Frequency>(query, pageIndex - 1, pageSize) as IPagedList<Frequency>);
            }
            );
        }

        public int GetMarkFrequencyByCode(string code)
        {
            var mark = 0;
            var frequency = _frequencyRepositoryAsync.Table.FirstOrDefault(x => x.Code.Equals(code));

            if (frequency != null)
                mark = frequency.Mark;

            return mark;
        }
    }
}
