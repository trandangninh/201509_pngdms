using Entities.Domain.ClassificationDefects;
using Entities.Domain.Suppliers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Service.Suppliers
{
    public interface IScMeasureTargetService : IBaseService<ScMeasureTarget>
    {
        /// <summary>
        /// get score card measure target by supplier identiy and scmeasure identity
        /// </summary>
        /// <param name="supplierId"></param>
        /// <param name="scMeasureId"></param>
        /// <returns></returns>
        Task<ScMeasureTarget> GetScMeasureTargetBySupplierIdAndScMeasureId(int supplierId, int scMeasureId);

        /// <summary>
        /// get all ScMeasureTarget by supplier identity
        /// </summary>
        /// <param name="supplierId"></param>
        /// <returns></returns>
        Task<List<ScMeasureTarget>> GetAllScMeasureTargetBySupplierId(int supplierId);
    }
}
