using Entities.Domain.ScMeasures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Service.ScMeasures
{
    public interface IScMeasureService : IBaseService<ScMeasure>
    {
        /// <summary>
        /// Get all ScMeasure
        /// </summary>
        /// <returns>paged list ScMeasure</returns>
        Task<IPagedList<ScMeasure>> GetAllScMeasureAsync(string searchBySCMeasureName = null, string searchBySCMeasureCode = null, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// check name of scMeasure existed or not
        /// </summary>
        /// <param name="code"></param>
        /// <returns>true or false</returns>
        Task<bool> CheckCodeHasExisted(string code);

        /// <summary>
        /// get scMeasure by code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<ScMeasure> GetScMeasureByCodeAsync(string code);

        /// <summary>
        /// get scMeasure by code
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<ScMeasure> GetScMeasureByNameAsync(string name);

        /// <summary>
        /// delete list scMeasures
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        Task DeleteScMeasuresAsync(List<int> listId);
    }
}
