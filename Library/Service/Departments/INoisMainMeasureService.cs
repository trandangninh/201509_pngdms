using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;

namespace Service.Interface
{
    public interface INoisMainMeasureService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hardCodeClass"></param>
        /// <returns></returns>
        Task CreateAsync(NoisMainMeasure hardCodeClass);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hardCodeClass"></param>
        /// <returns></returns>
        Task UpdateAsync(NoisMainMeasure hardCodeClass);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hardCodeClass"></param>
        /// <returns></returns>
        Task DeleteAsync(NoisMainMeasure hardCodeClass);


        /// <summary>
        /// List measure by date and line
        /// </summary>
        /// <param name="createdDate"></param>
        /// <param name="listLineCode"></param>
        /// <param name="listMeasureType"></param>
        /// <returns></returns>
        Task<List<NoisMainMeasure>> GetListMeasureByDateAndLineAndMeasure(DateTime createdDate, List<string> listLineCode,List<string> listMeasureType );

        NoisMainMeasure GetMainMeasureByLineCodeAndMeasureCodeAndDate(DateTime createdDate, string lineCode,
            string measureCode);
    }
}
