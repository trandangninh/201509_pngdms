using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;
using RepositoryPattern.Repositories;

namespace Repository.Implement
{

    public static class MeasureChainSupplyChain
    {
        public static Task<MeasureSupplyChain> GetMeasureSupplyChainByIdAsync(this IRepositoryAsync<MeasureSupplyChain> repository, int MeasureSupplyChainId)
        {
             if (MeasureSupplyChainId==0)
            {
                throw new ArgumentException("Null or empty argument: lineId");
            }
            return repository
                .Table
                .FirstOrDefaultAsync(x => x.Id == MeasureSupplyChainId);

        }
    }
}
