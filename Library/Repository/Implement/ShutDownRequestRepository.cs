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
   
    public static class ShutDownRequestRepository
    {
        public static Task<ShutdownRequest> GetShutdownRequestIdByIdAsync(this IRepositoryAsync<ShutdownRequest> repository, int shutdownRequestId)
        {
            if (shutdownRequestId == 0)
            {
                throw new ArgumentException("Null or empty argument: shutdownRequestId");
            }
            return repository
                .Table.Include(o => o.CreatedUser)
                .FirstOrDefaultAsync(x => x.Id == shutdownRequestId);

        }


    }
}
