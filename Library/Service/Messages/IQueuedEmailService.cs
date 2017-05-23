using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;


namespace Service.Messages
{
    public interface IQueuedEmailService
    {
        Task<List<QueuedEmail>> GetAll();

        List<QueuedEmail> GetAllNotAsync();


        QueuedEmail GetById(int queuedEmailId);
        Task CreateAsync(QueuedEmail queuedEmail);

        void CreateListAttachmentForEmail(QueuedEmail queuedEmail, List<string> listQueuedEmailAttachments);


        Task DeleteAsync(QueuedEmail queuedEmail);
        Task UpdateAsync(QueuedEmail queuedEmail);

        void UpdateNotAsync(QueuedEmail queuedEmail);
        void CreateNotAsync(QueuedEmail queuedEmail);

    }
}
