using Entities.Domain.QualityAlerts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryPattern.Repositories;
using Utils.Caching;

namespace Service.QualityAlerts
{
    public class SubColumnFormulaService : BaseService<SubColumnFormula>, ISubColumnFormulaService
    {
        private readonly IRepositoryAsync<SubColumnFormula> _subColumnFormulaRepository;
        private readonly ICacheManager _cacheManager;

        protected override string PatternKey
        {
            get
            {
                return "Nois.SubColumnFormula.";
            }
        }

        public SubColumnFormulaService(IRepositoryAsync<SubColumnFormula> subColumnFormulaRepository, ICacheManager cacheManager) : base(subColumnFormulaRepository, cacheManager)
        {
            this._subColumnFormulaRepository = subColumnFormulaRepository;
            this._cacheManager = cacheManager;
        }
    }
}
