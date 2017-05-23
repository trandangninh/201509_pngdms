using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Entities.Domain;
using RepositoryPattern.Repositories;
using Utils.Caching;

namespace Service.Messages
{
    public class QueuedEmailService :  IQueuedEmailService
    {

        private const string QUEUEDEMAIL_BY_ID_KEY = "Ef.queuedemail.byid-{0}";

        private const string QUEUEDEMAIL_All = "Ef.queuedemail.all";

        private const string QUEUEDEMAIL_PATTERN_KEY = "Ef.queuedemail.";

        private readonly IRepositoryAsync<QueuedEmail> _repository;
        private readonly IRepositoryAsync<QueuedEmailAttachment> _attachmentRepositoryAsync; 
        private readonly ICacheManager _cacheManager;

        public QueuedEmailService(IRepositoryAsync<QueuedEmailAttachment> attachmentRepositoryAsync,
            IRepositoryAsync<QueuedEmail> repository, ICacheManager cacheManager)
        {
            _repository = repository;
            _cacheManager = cacheManager;
            _attachmentRepositoryAsync = attachmentRepositoryAsync;
        }

        public Task<List<QueuedEmail>> GetAll()
        {
            var query = _repository.Table.Include(p=>p.QueuedEmailAttachments);

            return query.ToListAsync();
        }

        public List<QueuedEmail> GetAllNotAsync()
        {
            var query = _repository.Table.Include(p => p.QueuedEmailAttachments);

            return query.ToList();
        }

        public QueuedEmail GetById(int queuedEmailId)
        {
            if (queuedEmailId <= 0)
                return null;
            var key = string.Format(QUEUEDEMAIL_BY_ID_KEY, queuedEmailId);
            return _cacheManager.Get(key, () => _repository.GetById(queuedEmailId));
        }

        public Task CreateAsync(QueuedEmail queuedEmail)
        {
            _cacheManager.RemoveByPattern(QUEUEDEMAIL_PATTERN_KEY);
            return _repository.InsertAsync(queuedEmail);
        }

        public void CreateNotAsync(QueuedEmail queuedEmail)
        {
            _cacheManager.RemoveByPattern(QUEUEDEMAIL_PATTERN_KEY);
            _repository.Insert(queuedEmail);
        }


        public void CreateListAttachmentForEmail(QueuedEmail queuedEmail,
            List<string> listQueuedEmailAttachments)
        {
            if(listQueuedEmailAttachments==null) return;
            foreach (var attach in listQueuedEmailAttachments)
            {
                var newAttachment = new QueuedEmailAttachment()
                {
                    FilePath = attach,
                    QueuedEmailId = queuedEmail.Id
                };
                _attachmentRepositoryAsync.Insert(newAttachment);
            }
        }


        public Task UpdateAsync(QueuedEmail queuedEmail)
        {
            _cacheManager.RemoveByPattern(QUEUEDEMAIL_PATTERN_KEY);
            return _repository.UpdateAsync(queuedEmail);
        }

        public void UpdateNotAsync(QueuedEmail queuedEmail)
        {
            _cacheManager.RemoveByPattern(QUEUEDEMAIL_PATTERN_KEY);
            _repository.Update(queuedEmail);
        }

        public Task DeleteAsync(QueuedEmail queuedEmail)
        {
            _cacheManager.RemoveByPattern(QUEUEDEMAIL_PATTERN_KEY);
            return _repository.DeleteAsync(queuedEmail);
        }
    }
}
