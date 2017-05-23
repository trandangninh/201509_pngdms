using Entities.Domain.Frequencys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Service.Frequencys
{
    public interface IFrequencyService : IBaseService<Frequency>
    {
        /// <summary>
        /// Get All Frequency
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<IPagedList<Frequency>> GetAllFrequencyAsync(int pageIndex = 0, int pageSize = int.MaxValue);

        int GetMarkFrequencyByCode(string code);
    }
}
