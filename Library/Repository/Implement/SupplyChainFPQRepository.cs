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

    public static class SupplyChainDDSRepository
    {
        public static Task<SupplyChainDDS> GetSupplyChainDDSByIdAsync(this IRepositoryAsync<SupplyChainDDS> repository, int SupplyChainDDSId)
        {

            if (SupplyChainDDSId == 0)
            {
                throw new ArgumentException("Null or empty argument: SupplyChainDDSId");
            }
            return repository
                .Table
                .FirstOrDefaultAsync(x => x.Id == SupplyChainDDSId);

        }
    }
}
