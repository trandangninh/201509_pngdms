using System;
using System.Collections.Generic;
using Entities.Domain;
using Service.Interface;

namespace Service.Tasks
{
    public class IssuesTask : ITask
    {
      
        private readonly IIssueService _issueService;

        public IssuesTask(IIssueService issueService)
        {
            _issueService = issueService;
        }

        public void Execute()
        {
            var yesterday = DateTime.Now.AddDays(-1).Date;
            var listIssues =  _issueService.SearchIssues(createdDate: yesterday, oldDate:true,statusId: new List<int>{(int)IssueStatus.Open} ).Result;
            foreach (var item in listIssues)
            {
                item.IssueStatus = IssueStatus.Delayed;
                _issueService.UpdateNotAsync(item);
            }
        }
    }
}
