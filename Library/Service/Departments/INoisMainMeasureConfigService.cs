using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;

namespace Service.Interface
{
    public interface INoisMainMeasureConfigService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hardCodeClass"></param>
        /// <returns></returns>
        Task CreateAsync(NoisMainMeasureConfig hardCodeClass);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hardCodeClass"></param>
        /// <returns></returns>
        Task UpdateAsync(NoisMainMeasureConfig hardCodeClass);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hardCodeClass"></param>
        /// <returns></returns>
        Task DeleteAsync(NoisMainMeasureConfig hardCodeClass);


        /// <summary>
        /// List measure by date and line
        /// </summary>
     
        /// <param name="listLineCode"></param>
        /// <param name="listMeasureType"></param>
        /// <returns></returns>
        Task<List<NoisMainMeasureConfig>> GetListMeasureLineAndMeasure( List<string> listLineCode,List<string> listMeasureType );

        NoisMainMeasureConfig GetMainMeasureByLineCodeAndMeasureCode( string lineCode,
            string measureCode);
    }
}
