using Entities.Domain.Dds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dds
{
    public partial interface IDdsConfigService : IBaseService<DdsConfig>
    {
        List<DdsConfig> GetDdsConfigByDepartmentId(int departmentId);
    }
}
