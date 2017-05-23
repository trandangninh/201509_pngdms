using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;

namespace Entities.Mapping
{
    public class QueuedEmailAttachmentMapping : EntityTypeConfiguration<QueuedEmailAttachment>
    {
        public QueuedEmailAttachmentMapping()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            // Table & Column Mappings
            this.ToTable("QueuedEmailAttachment");
            this.HasRequired(p => p.QueuedEmail).WithMany(o => o.QueuedEmailAttachments).HasForeignKey(p => p.QueuedEmailId);
        }
    }
}
