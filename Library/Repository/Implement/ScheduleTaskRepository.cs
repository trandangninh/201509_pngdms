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
    public static class ScheduleTaskRepository
    {
        public static Task<ScheduleTask> GetScheduleTaskByIdAsync(this IRepositoryAsync<ScheduleTask> repository, int id)
        {

            if (id <= 0)
            {
                throw new ArgumentException("Null or empty argument: id");
            }
            return repository
                .Table
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        public static ScheduleTask GetScheduleTaskById(this IRepositoryAsync<ScheduleTask> repository, int id)
        {

            if (id <= 0)
            {
                throw new ArgumentException("Null or empty argument: id");
            }
            return repository
                .Table
                .FirstOrDefault(x => x.Id == id);
        }


    }
}
