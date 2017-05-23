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
    
    public static class TrackingRepository
    {
        public static Task<Tracking> GetTrackingByIdAsync(this IRepositoryAsync<Tracking> repository, int TrackingId)
        {

            if (TrackingId == 0)
            {
                throw new ArgumentException("Null or empty argument: TrackingId");
            }
            return repository
                .Table
                .FirstOrDefaultAsync(x => x.Id == TrackingId);

        }
    }
}
