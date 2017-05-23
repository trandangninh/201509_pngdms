using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Entities.Domain;
using RepositoryPattern.Repositories;
using Utils.Caching;

namespace Service.Lines
{
    public class LineRemarkService : BaseService<LineRemark>, ILineRemarkService
    {
        protected override string PatternKey
        {
            get { return "PG.LineRemark"; }
        }

        #region Constants

        private const string LINEREMARK_BY_DATE_LINECODE_KEY = "PG.LineRemark.byid-{0}-{1}";

        #endregion

        private readonly ICacheManager _cacheManager;
        private readonly IRepositoryAsync<LineRemark> _lineRemarkRepositoryAsync;

        public LineRemarkService(ICacheManager cacheManager, 
            IRepositoryAsync<LineRemark> lineRemarkRepositoryAsync) : base(lineRemarkRepositoryAsync, cacheManager)
        {
            _cacheManager = cacheManager;
            _lineRemarkRepositoryAsync = lineRemarkRepositoryAsync;
        }

        public Task<LineRemark> GetLineByDateAndLineCode(DateTime date, string lineCode, int typeCode)
        {
            if (String.IsNullOrEmpty(lineCode))
                return null;
            var key = string.Format(LINEREMARK_BY_DATE_LINECODE_KEY, date, lineCode);
            return _cacheManager.Get(key, () => _lineRemarkRepositoryAsync.Table.FirstOrDefaultAsync(p => p.LineCode == lineCode && p.CreateDate == date && p.LineRemarkTypeId == typeCode));
    
        }
    }
}
