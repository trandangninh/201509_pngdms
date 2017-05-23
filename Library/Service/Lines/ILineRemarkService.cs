using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Domain;

namespace Service.Lines
{
    public interface ILineRemarkService : IBaseService<LineRemark>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<LineRemark> GetLineByDateAndLineCode(DateTime date, string lineCode,int typeCode);
    }
}
