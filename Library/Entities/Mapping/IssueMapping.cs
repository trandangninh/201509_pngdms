using System.Data.Entity.ModelConfiguration;
using Entities.Domain;

namespace Entities.Mapping
{
    public class IssueMapping : EntityTypeConfiguration<Issue>
    {
        public IssueMapping()
        {
            this.ToTable("Issue");
            this.HasKey(t => t.Id);
            this.Ignore(i => i.IssueStatus);

            this.HasRequired(i => i.Department)
                .WithMany()
                .HasForeignKey(i => i.DepartmentId);

            this.HasRequired(p => p.User)
                .WithMany(u=>u.CreatedIssues)
                .HasForeignKey(p => p.UserId)
                .WillCascadeOnDelete(false);

            this.HasRequired(p => p.UserOwner)
                .WithMany(o => o.AsignedIssues)
                .HasForeignKey(p => p.UserOwnerId)
                .WillCascadeOnDelete(false);
        }
    }
}
