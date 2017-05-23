using System.Collections.Generic;
using Entities.Domain;

namespace Service.Messages
{
    public interface ISendMailService
    {

        bool Sendmail(QueuedEmail queuedEmail, List<string> listAttachmentFiles);


    }
}
