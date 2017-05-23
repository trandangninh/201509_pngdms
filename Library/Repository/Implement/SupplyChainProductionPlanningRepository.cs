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

    public static class SupplyChainProductionPlanningRepository
    {
        public static Task<SupplyChainProductionPlanning> GetSupplyChainProductionPlanningByIdAsync(this IRepositoryAsync<SupplyChainProductionPlanning> repository, int SupplyChainProductionPlanningId)
        {

            if (SupplyChainProductionPlanningId == 0)
            {
                throw new ArgumentException("Null or empty argument: SupplyChainProductionPlanningId");
            }
            return repository
                .Table
                .FirstOrDefaultAsync(x => x.Id == SupplyChainProductionPlanningId);

        }
    }
}
