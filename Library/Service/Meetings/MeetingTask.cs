using System;
using System.Linq;
using Service.Tasks;

namespace Service.Meetings
{
    public partial class MeetingTask : ITask
    {
        private readonly IMeetingService _meetingService;
        public MeetingTask(IMeetingService meetingService)
        {
            this._meetingService = meetingService;
        }

        public void Execute()
        {
            var meetings = _meetingService.GetAllAsync();
            foreach (var meeting in meetings.Result)
            {
                if (meeting.CurrentLeaderId > 0)
                {
                    // find current in meeting
                    var currentLeaderId = meeting.CurrentLeaderId;
                    var currentLeader = meeting.UserInMeetings.FirstOrDefault(x => x.UserId == currentLeaderId);
                    if(currentLeader == null)
                        continue;
                    var query = meeting.UserInMeetings.Where(um => um.IsLeader).OrderBy(um => um.Order);

                    var nextLeader = query.FirstOrDefault(um => um.Order > currentLeader.Order) ??
                                     query.FirstOrDefault();

                    if (nextLeader == null)
                        continue;

                    _meetingService.UpdateMeetingLeader(meeting.Id, meeting.CurrentLeaderId, nextLeader.UserId);

                    meeting.CurrentLeaderId = nextLeader.UserId;
                    meeting.UpdateCurrentLeaderDate = DateTime.Now.Date;
                    _meetingService.UpdateAsync(meeting);
                }
            }
        }
    }
}
