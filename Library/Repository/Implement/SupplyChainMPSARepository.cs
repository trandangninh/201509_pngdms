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

    public static class SupplyChainMPSARepository
    {
        public static Task<SupplyChainMPSA> GetSupplyChainMPSAByIdAsync(this IRepositoryAsync<SupplyChainMPSA> repository, int SupplyChainMPSAId)
        {

            if (SupplyChainMPSAId == 0)
            {
                throw new ArgumentException("Null or empty argument: SupplyChainMPSAId");
            }
            return repository
                .Table
                .FirstOrDefaultAsync(x => x.Id == SupplyChainMPSAId);

        }
    }
}
