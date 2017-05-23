using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Entities.Domain;
using RepositoryPattern.Repositories;

namespace Service.Common
{
    public class ScheduleTaskService : IScheduleTaskService
    {
        private readonly IRepositoryAsync<ScheduleTask> _scheduleTaskRepositoryAsync;

        public ScheduleTaskService(IRepositoryAsync<ScheduleTask> scheduleTaskRepositoryAsync)

        {
            _scheduleTaskRepositoryAsync = scheduleTaskRepositoryAsync;
        }

        public Task DeleteTask(ScheduleTask task)
        {
            return _scheduleTaskRepositoryAsync.DeleteAsync(task);

        }

        public Task<ScheduleTask> GetTaskById(int taskId)
        {
            return _scheduleTaskRepositoryAsync.GetByIdAsync(taskId);
        }

        public Task<ScheduleTask> GetTaskByType(string type)
        {
            return _scheduleTaskRepositoryAsync.Table.FirstOrDefaultAsync(p => p.Type == type);
        }

        public List<ScheduleTask> GetAllTasks(bool showHidden = false)
        {
            return _scheduleTaskRepositoryAsync.Table.ToList();
        }

        public Task CreateTaskAsync(ScheduleTask task)
        {
            return _scheduleTaskRepositoryAsync.InsertAsync(task);

        }

        public Task UpdateTaskAsync(ScheduleTask task)
        {
            return _scheduleTaskRepositoryAsync.UpdateAsync(task);

        }
    }
}
