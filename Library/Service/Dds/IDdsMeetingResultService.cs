using Entities.Domain.Dds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dds
{
    public partial interface IDdsMeetingResultService : IBaseService<DdsMeetingResult>
    {
        Task<DdsMeetingResult> GetDdsMeetingResultByMeasureIdAndLineIdAndDate(int measureId, int lineId, DateTime date);
    }
}
