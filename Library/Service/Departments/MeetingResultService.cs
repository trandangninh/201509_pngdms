using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;
using RepositoryPattern.Repositories;
using Service.Interface;

namespace Service.Implement
{
    public class MeetingResultService:IMeetingResultService
    {
        private readonly IRepositoryAsync<MeetingResult> _meetingResultRepositoryAsyc; 
        public MeetingResultService(IRepositoryAsync<MeetingResult> repository) 
        {
            _meetingResultRepositoryAsyc = repository;
        }

        public Task<MeetingResult> GetMeetingResultById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<MeetingResult>> GetAllMeetingResults()
        {
            throw new NotImplementedException();
        }

        public Task<List<MeetingResult>> SearchMeetingResults(DateTime? searchDate, int? lineId, string userId = null)
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(MeetingResult meetingResult)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(MeetingResult meetingResult)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(MeetingResult meetingResult)
        {
            throw new NotImplementedException();
        }
    }
}
