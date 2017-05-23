using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain.Dds;

namespace Service.Dds
{
    public interface IDdsMeetingPrDetailService : IBaseService<DdsMeetingPrDetail>
    {
        Task<DdsMeetingPrDetail> GetByDdsMeetingResult(int ddsMeetingResult);
    }
}
