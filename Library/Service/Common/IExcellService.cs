using System;
using System.Data;

namespace Service.Common
{
    public interface IExcellService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="hasHeader"></param>
        /// <returns></returns>
        DataTable ReadExcellToDataTable(string url, Boolean hasHeader);
    }
}
