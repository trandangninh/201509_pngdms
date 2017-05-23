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

    public static class UserAllowInSupplyChainRepository
    {
        public static Task<UserAllowInSupplyChain> GetTrackingByIdAsync(this IRepositoryAsync<UserAllowInSupplyChain> repository, int userAllowInSupplyChainId)
        {

            if (userAllowInSupplyChainId == 0)
            {
                throw new ArgumentException("Null or empty argument: TrackingId");
            }
            return repository
                .Table
                .FirstOrDefaultAsync(x => x.Id == userAllowInSupplyChainId);

        }
    }
}
