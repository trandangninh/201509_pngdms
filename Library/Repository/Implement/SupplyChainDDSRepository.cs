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

    public static class SupplyChainFPQRepository
    {
        public static Task<SupplyChainFPQ> GetSupplyChainFPQByIdAsync(this IRepositoryAsync<SupplyChainFPQ> repository, int SupplyChainFPQId)
        {

            if (SupplyChainFPQId == 0)
            {
                throw new ArgumentException("Null or empty argument: SupplyChainFPQId");
            }
            return repository
                .Table
                .FirstOrDefaultAsync(x => x.Id == SupplyChainFPQId);

        }
    }
}
