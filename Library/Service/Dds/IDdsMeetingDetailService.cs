using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain.Dds;

namespace Service.Dds
{
    public interface IDdsMeetingDetailService : IBaseService<DdsMeetingDetail>
    {
        Task<DdsMeetingDetail> GetByDdsMeetingResult(int ddsMeetingResult);
    }
}
