using Entities.Domain.Departments;
using Entities.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Service.Departments
{
    public partial interface IDepartmentService : IBaseService<Department>
    {
        IPagedList<Department> SearchDepartment(User user = null, bool? isActive = null, bool includeSupplyChain = true, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Get department by department name
        /// </summary>
        /// <param name="departmentName">departmentName</param>
        /// <returns>Department</returns>
        Task<Department> GetDepartmentByDepartmentName(string departmentName);
        /// <summary>
        /// Get supply chain department
        /// </summary>
        /// <returns></returns>
        Task<Department> GetSupplyChainDepartment();
        /// <summary>
        /// Get department by list of ids
        /// </summary>
        /// <param name="ids">List of department identity</param>
        /// <returns></returns>
        Task<List<Department>> GetDepartmentByIdsAsync(List<int> ids);
    }
}
