using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Entities.Domain.Classifications;
using Utils;
using System.Linq;

namespace Service.QualityAlerts
{
    public partial interface IClassificationService : IBaseService<Classification>
    {
        /// <summary>
        /// Get Classification by Classification code
        /// </summary>
        /// <param name="classificationCode">Classification Code</param>
        /// <returns></returns>
        Task<Classification> GetClassificationByClassificationCode(string classificationCode);

        IPagedList<ClassificationFullObject> GetList(int pageIndex = 0, int pageSize = int.MaxValue);
    }
}
