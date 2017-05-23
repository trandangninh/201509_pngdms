using Entities.Domain.ClassificationDefects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ClassificationDefects
{
    public interface IMaterialService
    {
        /// <summary>
        /// get all materials
        /// </summary>
        /// <returns>list material</returns>
        IList<Material> GetAllMaterials();

        /// <summary>
        /// get material by material identity
        /// </summary>
        /// <param name="materialId"></param>
        /// <returns>Material</returns>
        Material GetMaterialById(int materialId);
    }
}
