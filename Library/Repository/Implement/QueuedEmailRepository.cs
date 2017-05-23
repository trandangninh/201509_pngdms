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
    public static class QueuedEmailRepository
    {

        public static Task<QueuedEmail> GetByIdAsync(this IRepositoryAsync<QueuedEmail> repository, int id)
        {

            if (id <= 0)
            {
                throw new ArgumentException("Null or empty argument: id");
            }
            return repository
                .Table.Include(p => p.QueuedEmailAttachments)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        public static QueuedEmail GetById(this IRepositoryAsync<QueuedEmail> repository, int id)
        {

            if (id <= 0)
            {
                throw new ArgumentException("Null or empty argument: id");
            }
            return repository
                .Table.Include(p => p.QueuedEmailAttachments)
                .FirstOrDefault(x => x.Id == id);
        }
    }
}
