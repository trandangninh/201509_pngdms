using System;
using System.Linq;
using Entities.Domain;
using Entities.Domain.Dds;
using Service.Departments;
using Service.Lines;
using Service.Meetings;
using Service.QualityAlerts;
using Service.Tasks;
using Service.SupplyChain;
using Utils;
using Service.Users;

namespace Service.Dds
{
    public class DdsMeetingTask : ITask
    {
        private readonly IDdsConfigService _ddsConfigService;
        private readonly ILineService _lineService;
        private readonly IMeasureService _measureService;
        private readonly IDepartmentService _departmentService;
        private readonly IDdsMeetingService _ddsMeetingService;
        private readonly IQualityAlertService _qualityAlertService;
        private readonly IMeetingService _meetingService;
        private readonly ISupplyChainFPQService _supplyChainFPQ;
        private readonly IWorkContext _workContext;
        private readonly IUserService _userService;
        public DdsMeetingTask(IDdsConfigService ddsConfigService,
            ILineService lineService,
            IMeasureService measureService,
            IDepartmentService departmentService,
            IDdsMeetingService ddsMeetingService,
            IQualityAlertService qualityAlertService,
            IMeetingService meetingService,
            ISupplyChainFPQService supplyChainFPQ,
            IWorkContext workContext,
            IUserService userService
            )
        {
            _ddsConfigService = ddsConfigService;
            _lineService = lineService;
            _measureService = measureService;
            _departmentService = departmentService;
            _ddsMeetingService = ddsMeetingService;
            _qualityAlertService = qualityAlertService;
            _meetingService = meetingService;
            _supplyChainFPQ = supplyChainFPQ;
            _workContext = workContext;
            _userService = userService;
        }

        public void Execute()
        {
            //if (DateTime.Now.TimeOfDay < new TimeSpan(11,0,0) ||
            //    DateTime.Now.TimeOfDay > new TimeSpan(11,30,0))
            //    return;

            if (DateTime.Now.TimeOfDay < new TimeSpan(7, 0, 0))
                return;

            var departments = _departmentService.SearchDepartment(null,true);

            var date = DateTime.Today;

             foreach (var department in departments)
            {
                var readyDdsMeeting = _ddsMeetingService.GetDdsMeetingByDate(date, department.Id).Result;
                if (readyDdsMeeting != null)
                    continue;

                //Next Leader
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    var meeting = _meetingService.GetMeetingByDepartmentId(department.Id).Result;
                    if (meeting != null && meeting.CurrentLeaderId > 0)
                    {
                        // find current in meeting
                        var currentLeaderId = meeting.CurrentLeaderId;
                        var currentLeader = meeting.UserInMeetings.FirstOrDefault(x => x.UserId == currentLeaderId);
                        if (currentLeader != null)
                        {
                            var query = meeting.UserInMeetings.Where(um => um.IsLeader).OrderBy(um => um.Order);

                            var nextLeader = query.FirstOrDefault(um => um.Order > currentLeader.Order) ?? query.FirstOrDefault();

                            if (nextLeader != null)
                            {
                                _meetingService.UpdateMeetingLeader(meeting.Id, meeting.CurrentLeaderId, nextLeader.UserId).Wait();

                                meeting.CurrentLeaderId = nextLeader.UserId;
                                meeting.UpdateCurrentLeaderDate = DateTime.Now.Date;
                                _meetingService.UpdateAsync(meeting).Wait();
                            }
                        }
                    }
                }
                //

                var ddsMeeting = new DdsMeeting
                {
                    DepartmentId = department.Id,
                    CreatedDateTime = date
                };
                _ddsMeetingService.InsertAsync(ddsMeeting).Wait();

                var ddsConfig = _ddsConfigService.GetDdsConfigByDepartmentId(department.Id);

                //var lines = _lineService.SearchLines(departmentId: department.Id, active:true).Result;
                var lines = _lineService.SearchLines(departmentId: department.Id).Result;
                var measures = _measureService.GetAllMeasureByDepartmentId(department.Id);

                foreach (var measure in measures)
                {
                    foreach (var line in lines)
                    {
                        var value = "";
                        if (measure.MeasureSystemType == MeasureSystemType.QuanlityAlert)
                        {
                            try
                            {
                                value = _qualityAlertService.CountQuanlityAllertByLineAndDate(line.Id, DateTime.Now).ToString();
                                UpdateQualityAlertToSupplyChain(line, value);
                            }
                            catch
                            {

                            }
                        }
                        if(line.Active)
                        {
                            var configuration = ddsConfig.FirstOrDefault(c => c.MeasureId == measure.Id && c.LineId == line.Id);

                            var ddsMeetingResult = new DdsMeetingResult
                            {
                                DdsMeetingId = ddsMeeting.Id,
                                MeasureId = measure.Id,
                                LineId = line.Id,
                                ReadOnly = configuration != null,
                                Result = value
                            };
                            ddsMeeting.DdsMeetingResults.Add(ddsMeetingResult);
                        }

                    }
                }

                _ddsMeetingService.UpdateAsync(ddsMeeting).Wait();
            }
        }

        private void UpdateQualityAlertToSupplyChain(Line line, string value)
        {
            var measureCode = 2;
            var createdDate = DateTime.Now;
            var supplychainResult = _supplyChainFPQ.GetSupplyChainFPQMeasureCodeAndDate(measureCode.ToString(), createdDate);
            //get last supply chain FPQ Result
            var lastsupplyChainFPQResult = _supplyChainFPQ.GetSupplyChainFPQMeasureCodeAndDate(measureCode.ToString(), createdDate.AddDays(-1));

            if (string.IsNullOrEmpty(value))
                value = "0";

            string owner = string.Empty;
            //if last supplychainFPQ result != null -> get it's owner
            if (lastsupplyChainFPQResult != null)
                owner = lastsupplyChainFPQResult.Owner;
            //if it == null -> get last user in list user to set owner(logic of "Quoc")
            else
            {
                var userOwner = _userService.GetAllUsersAsync().LastOrDefault();
                if (userOwner != null)
                    owner = userOwner.Username;

            }


            if (supplychainResult == null)
            {                
                var newModel = new Entities.Domain.SupplyChainFPQ()
                {

                    CreatedDate = createdDate,
                    UpdatedDate = createdDate,
                    MeasureCode = measureCode,
                    Owner = owner//this owner not show on FPQ SupplyChain
                };
                switch (line.LineCode)
                {
                    case "LPD1": newModel.LPD1 = value;
                        break;
                    case "LPD2": newModel.LPD2 = value;
                        break;
                    case "LPD3": newModel.LPD3 = value;
                        break;
                    case "FE Batch": newModel.Batch = value;
                        break;
                    case "Bottle": newModel.Bottle = value;
                        break;
                    case "Sachet1":
                         if (!string.IsNullOrEmpty(supplychainResult.Sachet) && int.Parse(supplychainResult.Sachet) >= 0)
                            supplychainResult.Sachet = (int.Parse(supplychainResult.Sachet) + int.Parse(value)).ToString();
                        else
                            supplychainResult.Sachet = value;
                        break;
                    case "Sachet2":
                         if (!string.IsNullOrEmpty(supplychainResult.Sachet) && int.Parse(supplychainResult.Sachet) >= 0)
                            supplychainResult.Sachet = (int.Parse(supplychainResult.Sachet) + int.Parse(value)).ToString();
                        else
                            supplychainResult.Sachet = value;
                        break;
                    case "Pouch": newModel.Pouch = value;
                        break;
                    case "FR": newModel.FR = value; newModel.FRMK = value;
                        break;
                }
                _supplyChainFPQ.CreateAsync(newModel).Wait();
                //these Owner will show on FPQ SupplyChain
                var listOwnerYesterday = _supplyChainFPQ.GetUserIdInSupplyChainFPQ(lastsupplyChainFPQResult.Id);
                foreach (var itemOwner in listOwnerYesterday)
                {
                    _supplyChainFPQ.CreateUserInSupplyChainFPQAsync(new UserInSupplyChainFpq()
                    {
                        SupplyChainFpqId =
                            newModel.Id,
                        UserId = itemOwner
                    }).Wait();
                }
            }
            else
            {
                supplychainResult.UpdatedDate = createdDate;
                supplychainResult.MeasureCode = measureCode;
                supplychainResult.Owner = supplychainResult.Owner == null ? owner : supplychainResult.Owner;
                switch (line.LineCode)
                {
                    case "LPD1": supplychainResult.LPD1 = value;
                        break;
                    case "LPD2": supplychainResult.LPD2 = value;
                        break;
                    case "LPD3": supplychainResult.LPD3 = value;
                        break;
                    case "FE Batch": supplychainResult.Batch = value;
                        break;
                    case "Bottle": supplychainResult.Bottle = value;
                        break;
                    case "Sachet1":
                        if (!string.IsNullOrEmpty(supplychainResult.Sachet) && int.Parse(supplychainResult.Sachet) >= 0)
                            supplychainResult.Sachet = (int.Parse(supplychainResult.Sachet) + int.Parse(value)).ToString();
                        else
                            supplychainResult.Sachet = value;
                        break;
                    case "Sachet2":
                        if (!string.IsNullOrEmpty(supplychainResult.Sachet) && int.Parse(supplychainResult.Sachet) >= 0)
                            supplychainResult.Sachet = (int.Parse(supplychainResult.Sachet) + int.Parse(value)).ToString();
                        else
                            supplychainResult.Sachet = value;
                        break;
                    case "Pouch": supplychainResult.Pouch = value;
                        break;
                    case "FR": supplychainResult.FR = value; supplychainResult.FRMK = value;
                        break;
                }

                _supplyChainFPQ.UpdateAsync(supplychainResult).Wait();
            }

        }
    }
}
