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
    
    public static class MeetingResultRepository
    {
        public static Task<MeetingResult> GetMeetingResultByIdAsync(this IRepositoryAsync<MeetingResult> repository, int meetingResultId)
        {

            if (meetingResultId == 0)
            {
                throw new ArgumentException("Null or empty argument: meetingResultId");
            }
            return repository
                .Table
                .FirstOrDefaultAsync(x => x.Id == meetingResultId);

        }

        public static Task<List<MeetingResult>> GetMeetingResultsByTimeAsync(this IRepositoryAsync<MeetingResult> repository, DateTime createdDate)
        {
            var beginDate = new DateTime(createdDate.Year, createdDate.Month, createdDate.Day);
            var endDate = beginDate.AddDays(1);
            return repository
                .Table
                .Where(x => x.CreatedDate.Date >= beginDate && x.CreatedDate < endDate).ToListAsync();

        }

        public static Task<List<MeetingResult>> GetMeetingResultsByLineAsync(this IRepositoryAsync<MeetingResult> repository, int lineId)
        {
            if (lineId == 0)
            {
                throw new ArgumentException("Null or empty argument: lineId");
            }
            return repository
                .Table
                .Where(x => x.LineId == lineId).ToListAsync();

        }


    }
}
