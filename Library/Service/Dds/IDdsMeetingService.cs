using Entities.Domain.Dds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Service.Dds
{
    public partial interface IDdsMeetingService : IBaseService<DdsMeeting>
    {
        Task<DdsMeeting> GetDdsMeetingByDate(DateTime date, int departmentId);
        Task<IPagedList<DdsMeeting>> GetDdsMeetingByDate(DateTime fromDate, DateTime toDate, int departmentId = 0, int pageIndex = 0, int pageSize = int.MaxValue);
        string AddLineRemarkToXmlData(string lineRemarkXml, int lineId, string remark);
        Dictionary<int, string> LineRemarkParser(string lineRemarkXml);
    }
}
