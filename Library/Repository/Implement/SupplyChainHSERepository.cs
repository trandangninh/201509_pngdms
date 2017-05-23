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

    public static class SupplyChainHSERepository
    {
        public static Task<SupplyChainHSE> GetSupplyChainHSEByIdAsync(this IRepositoryAsync<SupplyChainHSE> repository, int SupplyChainHSEId)
        {

            if (SupplyChainHSEId == 0)
            {
                throw new ArgumentException("Null or empty argument: SupplyChainHSEId");
            }
            return repository
                .Table
                .FirstOrDefaultAsync(x => x.Id == SupplyChainHSEId);

        }
    }
}
