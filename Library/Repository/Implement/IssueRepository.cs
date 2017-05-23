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
   
    public static class IssueRepository
    {
        public static Task<Issue> GetIssueByIdAsync(this IRepositoryAsync<Issue> repository, int issueId)
        {
            if (issueId == 0)
            {
                throw new ArgumentException("Null or empty argument: Issue");
            }
            return repository
                .Table.Include(o=>o.User)
                .FirstOrDefaultAsync(x => x.Id == issueId);

        }


    }
}
