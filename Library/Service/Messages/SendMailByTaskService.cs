using System.Collections.Generic;
using Entities.Domain;

namespace Service.Messages
{
    public class SendMailByTaskService : ISendMailService
    {
        private readonly IQueuedEmailService _queuedEmailService;

        public SendMailByTaskService(IQueuedEmailService queuedEmailService)
        {
            _queuedEmailService = queuedEmailService;
        }

        public bool Sendmail(QueuedEmail queuedEmail,List<string> listAttachmentFiles)
        {
            _queuedEmailService.CreateNotAsync(queuedEmail);
            _queuedEmailService.CreateListAttachmentForEmail(queuedEmail, listAttachmentFiles);
            return true;
        }
    }
}
