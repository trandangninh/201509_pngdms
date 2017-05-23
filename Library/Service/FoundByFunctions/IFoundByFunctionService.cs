using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain.FoundByFunction;
using Utils;

namespace Service.FoundByFunctions
{
    public partial interface IFoundByFunctionService : IBaseService<FoundByFunction>
    {
        Task<FoundByFunction> GetFoundByFunctionByName(string Name);
        Task<IPagedList<FoundByFunction>> GetListFoundByFunctions(int pageIndex = 0, int pageSize = int.MaxValue);
    }
}
