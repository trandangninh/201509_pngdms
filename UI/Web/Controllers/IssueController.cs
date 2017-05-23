using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Entities.Domain;
using Entities.Domain.Users;
using Service.Interface;
using Service.Messages;
using Service.Users;
using Utils;
using Web.Extend;
using Web.Models.Packing;
using Service.Departments;
using Entities.Domain.Departments;
using Service.Security;
using Nois.Web.Framework.Kendoui;

namespace Web.Controllers
{
    
    public class IssueController : Controller
    {
        private readonly IIssueService _issueService;
        private readonly IUserService _userService;
        private readonly ISendMailService _sendMailService;
        private readonly IWorkFlowMessageService _workFlowMessageService;
        private readonly IWorkContext _workContext;
        private readonly IDepartmentService _departmentService;
        private readonly IPermissionService _permissionService;
        public IssueController(IIssueService issueService, 
            IUserService userService, 
            ISendMailService sendMailService, 
            IWorkFlowMessageService workFlowMessageService, 
            IWorkContext workContext,
            IDepartmentService departmentService,
            IPermissionService permissionService)
        {
            _issueService = issueService;
            _userService = userService;
            _sendMailService = sendMailService;
            _workFlowMessageService = workFlowMessageService;
            _workContext = workContext;
            _departmentService = departmentService;
            this._permissionService = permissionService;
        }

        [HttpPost]
        public async Task<ActionResult> List(DataSourceRequest command, SearchIssueModel model)
        {
            //var type = (IssueType)model.Type;
            // Works
            DateTime dateSearch = DateTime.Parse(model.Datetime, CultureInfo.InvariantCulture);

            //var userId = _workContext.CurrentUser.IsAdmin() ? 0 : _workContext.CurrentUser.Id;
            //var issues = await _issueService.SearchIssues(new List<int>{userId}, searchKeyword:model.SearchKeyword, createdDate:dateSearch, departmentId:7);

            var includeUserCreated = false;
            List<int> users = null;
            //if (_workContext.CurrentUser != null && !_workContext.CurrentUser.IsAdmin())
            //{
            //    users = new List<int> { _workContext.CurrentUser.Id };
            //    includeUserCreated = true;
            //}
            //else
            //    users = String.IsNullOrEmpty(model.UserId) ? null : model.UserId.Split(',').Select(int.Parse).ToList();

            var issues = await _issueService.SearchIssues(false, users, includeUserCreated, searchKeyword: model.SearchKeyword, createdDate: dateSearch, departmentId: 7, pageIndex: command.Page - 1, pageSize: command.PageSize);

            var listIssueModel = issues.Select(p => new IssueModel()
            {
                Id = p.Id,
                Content = p.Content,
                CreatedDate = p.CreatedDate.ToShortDateString(),
                NextStep = p.NextStep,
                Status = p.IssueStatus.ToString(),
                StatusId = (int)p.IssueStatus,
                UserCreated = p.User.Username,
                UserAssigned = p.UserOwner.Username,
                UserAssignedId = p.UserOwnerId,
                //Type = p.IssueType.ToString(),
                ActionPlan = p.ActionPlan,
                When = p.When,
                Note = p.Note,
                SystemFixDMSLinkage = p.SystemFixDmsLinkage,
                WhenDue =p.WhenDue.ToShortDateString() 
            });

            var result = new DataSourceResult()
            {
                Data = listIssueModel, // Process data (paging and sorting applied)
                Total = issues.TotalCount // Total number of records
            };

            // Return the result as JSON
            return Json(result);
        }

        [HttpPost]
        public async Task<ActionResult> Create(IssueModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.WriteIssues))
                return Content("You don't have permission, please contact with Administrator to be supported");
            //var type = (IssueType)int.Parse(model.TypeId);
            var status = (IssueStatus)model.StatusId;

            var supplyChain = _departmentService.SearchDepartment(includeSupplyChain:true).FirstOrDefault(d=>d.DepartmentTypeId == (int)DepartmentType.SupplyChain);

            var newIssue = new Issue()
            {
                Content = model.Content,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                NextStep = model.NextStep,
                IssueStatus = status,
                UserId = _workContext.CurrentUser.Id,
                ActionPlan = model.ActionPlan,
                When = Extension.Parse(model.When,DateTime.Today).ToShortDateString(),
                Note = model.Note,
                SystemFixDmsLinkage = model.SystemFixDMSLinkage,
                UserOwnerId = model.UserAssignedId,
                WhenDue = Extension.Parse(model.WhenDue,DateTime.Today),
                DepartmentId = supplyChain == null? 0 : supplyChain.Id
            };
            await _issueService.InsertAsync(newIssue);
            
            await _userService.GetUserByIdAsync(model.UserAssignedId);
            
            var userMail = await _userService.GetUserByIdAsync(model.UserAssignedId);
            var listAttachment = new List<string>();
            var queueEmail1 = _workFlowMessageService.SendIssusToUser(userMail, newIssue);
            var result = _sendMailService.Sendmail(queueEmail1, listAttachment);

            return new NullJsonResult();
        }


        public async Task<ActionResult> Update(IssueModel model)
        {
            var issue = await _issueService.GetIssueById(model.Id);
            if (issue != null)
            {
                issue.Content = model.Content;
                issue.NextStep = model.NextStep;
                issue.IssueStatus = (IssueStatus)model.StatusId;
                issue.UpdatedDate = DateTime.Now;
                issue.ActionPlan = model.ActionPlan;
                issue.When = Extension.Parse(model.When, DateTime.Today).ToShortDateString();

                issue.WhenDue = Extension.Parse(model.WhenDue, DateTime.Today);
                issue.Note = model.Note;
                issue.SystemFixDmsLinkage = model.SystemFixDMSLinkage;
                await _issueService.UpdateAsync(issue);

                if (_workContext.CurrentUser.IsAdmin())
                {
                    var userMail = await _userService.GetUserByUsernameAsync(model.UserAssigned);
                    var listAttachment = new List<string>();
                    var queueEmail1 = _workFlowMessageService.SendIssusToUser(userMail, issue);
                    _sendMailService.Sendmail(queueEmail1, listAttachment);
                }
                else
                {
                    var userCreate = await _userService.GetUserByUsernameAsync(model.UserCreated);
                    var listAttachment = new List<string>();
                    var queueEmail1 = _workFlowMessageService.SendIssusToOwner(userCreate, issue, model.UserAssigned);
                    _sendMailService.Sendmail(queueEmail1, listAttachment);
                }
            }

            return new NullJsonResult();
        }


        public async Task<ActionResult> UpdateByEmployee(IssueModel model)
        {
            var status = (IssueStatus)model.StatusId;

            var actualDay = DateTime.Now;


            var issue = await _issueService.GetIssueById(model.Id);
            if (issue != null)
            {
                issue.IssueStatus = status;
                issue.UpdatedDate = actualDay;
                await _issueService.UpdateAsync(issue);
            }
            return new NullJsonResult();
        }

        //[HttpPost]
        //public async Task<ActionResult> Delete(int id)
        //{
        //    var issue = await _issueService.GetIssueById(id);
        //    if (issue == null)
        //        throw new ArgumentException("No line found with the specified id");
            
        //    await _issueService.DeleteAsync(issue);
        //    return new NullJsonResult();
        //}

        [HttpPost]
        public async Task<ActionResult> Delete(List<int> listId)
        {
            foreach(var id in listId)
            {
                var issue = await _issueService.GetIssueById(id);
                if (issue == null)
                    throw new ArgumentException("No issue found with the specified id");
            }
            await _issueService.DeleteIssuesAsync(listId);
            return Json(new
            {
                status = "success",
            });
        }

        public ActionResult Manage()
        {
            if(_workContext.CurrentUser==null)
                return View("ManageIssueGuest");

            if (_workContext.CurrentUser.IsAdmin())
                return View("ManageIssueAdmin");
            
            //if(_workContext.CurrentUser.IsEmployee())
                return View("ManageIssueEmployee");
        }


        [HttpPost]
        public async Task<ActionResult> ManageList(DataSourceRequest command, SearchIssueModel model)
        {
            var issues = new List<Issue>();
            
            if (_workContext.CurrentUser==null || _workContext.CurrentUser.IsAdmin())
            {
                issues = await _issueService.GetAllIssues();
                if (!String.IsNullOrEmpty(model.SearchKeyword))
                {
                    issues = issues.Where(p => (p.ActionPlan != null && p.ActionPlan.Contains(model.SearchKeyword)) || (p.Content != null && p.Content.Contains(model.SearchKeyword)))
                       .ToList();
                }
                if (!String.IsNullOrEmpty(model.UserId))
                {
                    var lstUser = Enumerable.Cast<int>(model.UserId.Split(',')).ToList();
                    issues = await _issueService.GetIssuesOfListUser(lstUser);
                    if (!String.IsNullOrEmpty(model.SearchKeyword))
                    {
                        issues = issues.Where(p => (p.ActionPlan != null && p.ActionPlan.Contains(model.SearchKeyword)) || (p.Content != null && p.Content.Contains(model.SearchKeyword)))
                           .ToList();
                    }
                }
                if (!String.IsNullOrEmpty(model.Status))
                {
                    var lstStatus = model.Status.Split(',');
                    var lstResult = new List<Issue>();
                    foreach (var item in lstStatus)
                    {
                        if (!String.IsNullOrEmpty(item))
                        {
                            var status = (IssueStatus)int.Parse(item);
                            var resultIssues = issues.Where(p => p.IssueStatus == status)
                                .ToList();
                            lstResult.AddRange(resultIssues);
                        }
                    }
                    issues = lstResult;
                }
                if (model.Type>0)
                {
                    //khang comment var type = (IssueType)model.Type;
                    //issues = issues.Where(p => p.IssueType == type)
                      // .ToList();
                }
            }
            else
            {
                issues = _workContext.CurrentUser.CreatedIssues.ToList();
                if (!String.IsNullOrEmpty(model.SearchKeyword))
                {
                    issues = issues.Where(p => (p.ActionPlan != null && p.ActionPlan.Contains(model.SearchKeyword)) || (p.Content != null && p.Content.Contains(model.SearchKeyword)))
                       .ToList();
                }
                if (!String.IsNullOrEmpty(model.Status))
                {
                    var lstStatus = model.Status.Split(',');
                    var lstResult = new List<Issue>();
                    foreach (var item in lstStatus)
                    {
                        if (!String.IsNullOrEmpty(item))
                        {
                            var status = (IssueStatus) int.Parse(item);
                            var resultIssues = issues.Where(p => p.IssueStatus == status)
                                .ToList();
                            lstResult.AddRange(resultIssues);
                        }
                    }
                    issues = lstResult;
                }
                if (model.Type > 0)
                {
                    //khang comment var type = (IssueType)model.Type;
                    //issues = issues.Where(p => p.IssueType == type)
                    //   .ToList();
                }
            }

            issues = issues.OrderByDescending(p => p.WhenDue).ToList();
            var listIssueModel = issues.AsEnumerable().Select((p,i) => new IssueModel()
            {
                Index = i+1,
                Id = p.Id,
                Content = p.Content,
                CreatedDate = p.CreatedDate.ToShortDateString(),
                UpdatedDate = p.UpdatedDate.ToShortDateString(),
                NextStep = p.NextStep,
                Status = p.IssueStatus.ToString(),
                StatusId = p.IssueStatusId,
                UserCreated = p.User.Username,
                UserAssigned = p.UserOwner.Username,
                UserAssignedId = p.UserOwnerId,
                //Type = p.IssueType.ToString(),
                //TypeId =p.IssueTypeId.ToString(),
                ActionPlan = p.ActionPlan,
                When = p.When,
                Note = p.Note,
                WhenDue = p.WhenDue.ToShortDateString(),
                SystemFixDMSLinkage = p.SystemFixDmsLinkage ,
                IssuesPriority = p.IssuesPriority
            }).AsQueryable();

          
           

            // Calculate the total number of records before paging
            var total = listIssueModel.Count();


            // Apply paging
            if (command.Page > 0)
            {
                listIssueModel = listIssueModel.Skip((command.Page - 1) * command.PageSize);
            }
            listIssueModel = listIssueModel.Take(command.PageSize);

            var result = new DataSourceResult()
            {
                Data = listIssueModel.AsEnumerable(), // Process data (paging and sorting applied)
                Total = total // Total number of records
            };

            // Return the result as JSON
            return Json(result);
        }

        public async Task<ActionResult> ManageCreate(IssueModel model)
        {

            //var type = (IssueType)int.Parse(model.TypeId);
            var status = (IssueStatus)model.StatusId;
            var actualDay = DateTime.Now;
            
            var when = DateTime.Now;
            var whenDue = DateTime.Now;

            //baseDate = Convert.ToDateTime(model.BasePlanDate);
            if (model.WhenDue != null)
                whenDue = DateTime.ParseExact(model.WhenDue.Substring(0, 24),
                                   "ddd MMM d yyyy HH:mm:ss",
                                   CultureInfo.InvariantCulture);
         
            var newIssue = new Issue()
            {
                Content = model.Content,
                CreatedDate = actualDay,
                UpdatedDate = actualDay,
                NextStep = model.NextStep,
                IssueStatus = status,
                //IssueType = type,
                UserId = _workContext.CurrentUser.Id,
                ActionPlan = model.ActionPlan,
                Note = model.Note,
                WhenDue = whenDue.Date,
                SystemFixDmsLinkage = model.SystemFixDMSLinkage,
                IssuesPriority = model.IssuesPriority,
                UserOwnerId = model.UserAssignedId
            };


            if (model.date != null)
            {
                newIssue.UpdatedDate = DateTime.Parse(model.date,CultureInfo.InvariantCulture);
                newIssue.CreatedDate = DateTime.Parse(model.date,CultureInfo.InvariantCulture);
            }
            //baseDate = Convert.ToDateTime(model.BasePlanDate);
            if (model.When != null)
            {
                when = DateTime.ParseExact(model.When.Substring(0, 24),
                    "ddd MMM d yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture);
                newIssue.When = when.ToShortDateString();
            }
            await _issueService.InsertAsync(newIssue);

             var user = await _userService.GetUserByIdAsync(model.UserAssignedId);
            if (user != null)
            {
                var listAttachment = new List<string>();
                var issue = await _issueService.GetIssueById(newIssue.Id);
                var queueEmail1 = _workFlowMessageService.SendIssusToUser(user, issue);
                if (!User.IsInRole("Admin"))
                    queueEmail1 = _workFlowMessageService.SendIssusToOwner(_workContext.CurrentUser, issue, DateTime.Now.ToShortDateString());
                _sendMailService.Sendmail(queueEmail1, listAttachment);
            }

            return new NullJsonResult();
        }


        public async Task<ActionResult> ManageUpdate(IssueModel model)
        {
            var baseDate = new DateTime();
            if (!String.IsNullOrEmpty(model.When) )
            {
                if (model.When.Length > 10)
                    baseDate = DateTime.ParseExact(model.When.Substring(0, 24),
                        "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }

            DateTime whenDue;
            if (model.WhenDue.Length > 10)
                whenDue = DateTime.ParseExact(model.WhenDue.Substring(0, 24),
                                   "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            else
            {
                whenDue = Convert.ToDateTime(model.WhenDue);
            }

            //var type = (IssueType)int.Parse(model.TypeId);
            var status = (IssueStatus)model.StatusId;
            var actualDay = DateTime.Now;
            var issue = await _issueService.GetIssueById(model.Id);
            if (issue != null)
            {
                issue.Content = model.Content;
                issue.NextStep = model.NextStep;
                issue.IssueStatus = status;
                issue.UpdatedDate = actualDay;
                //issue.IssueType = type;
                issue.ActionPlan = model.ActionPlan;
                if (!String.IsNullOrEmpty(model.When))
                {
                    if (model.When.Length > 10)
                        issue.When = baseDate.ToShortDateString();
                }
                else
                    issue.When = "";
                issue.Note = model.Note;
                issue.WhenDue = whenDue;
                issue.SystemFixDmsLinkage = model.SystemFixDMSLinkage;
                issue.IssuesPriority = model.IssuesPriority;
                await _issueService.UpdateAsync(issue);

                var user = await _userService.GetUserByIdAsync(model.UserAssignedId);
                if (user != null)
                {
                    var listAttachment = new List<string>();

                    var queueEmail1 = _workFlowMessageService.SendIssusToUser(user, issue);
                    if (!User.IsInRole("Admin"))
                        queueEmail1 = _workFlowMessageService.SendIssusToOwner(issue.User, issue, DateTime.Now.ToShortDateString());
                    _sendMailService.Sendmail(queueEmail1, listAttachment);
                }
            }

            return new NullJsonResult();
        }


        public async Task<ActionResult> ManageUpdateByEmployee(IssueModel model)
        {
            var status = (IssueStatus)model.StatusId;
            var actualDay = DateTime.Now;
            var issue = await _issueService.GetIssueById(model.Id);
            if (issue != null)
            {

                issue.IssueStatus = status;
                issue.UpdatedDate = actualDay;
                await _issueService.UpdateAsync(issue);
                var userCreate = await _userService.GetUserByUsernameAsync(model.UserCreated);
                var listAttachment = new List<string>();
                var queueEmail1 = _workFlowMessageService.SendIssusToUser(userCreate, issue);
                if (!User.IsInRole("Admin"))
                    queueEmail1 = _workFlowMessageService.SendIssusToOwner(issue.User, issue, DateTime.Now.ToShortDateString());
                _sendMailService.Sendmail(queueEmail1, listAttachment);
            }
            return new NullJsonResult();
        }


        public ActionResult GetIssuePartial()
        {
            var issues = new List<Issue>();
            //if (_workContext.CurrentUser.IsAdmin())
            //{
            //    issues = _issueService.GetAllIssuesNotAsync();
            //}
            //else
            //{
            //    issues = _workContext.CurrentUser.AsignedIssues.ToList();
            //}

            issues = issues.Where(p => p.IssueStatus == IssueStatus.Open).OrderByDescending(p => p.UpdatedDate).ToList();
            var model = new IssueBarModel();
            var countOpenIssue = issues.Count;
            model.CountOpenIssue = countOpenIssue;
            var issueSh = issues.Take(5);
            if (issueSh.Any())
            {
                var issueShortModels = issueSh.ToList().Select(p => new ShortIssueModel
                {
                    Content = p.Content,
                    UpdateDateTime = p.UpdatedDate.ToShortDateString()
                }).ToList();
                model.ListShortModel = issueShortModels;
            }
            return PartialView("IssueBar", model);
        }
    }

    public class IssueBarModel
    {
        public List<ShortIssueModel> ListShortModel { get; set; }
        public int CountOpenIssue { get; set; }

        public IssueBarModel()
        {
            ListShortModel = new List<ShortIssueModel>();
        }
    }
    public class ShortIssueModel
    {
        public string UpdateDateTime { get; set; }
        public string Content { get; set; }
    }

    public class SearchIssueModel
    {
        public string Datetime { get; set; }
        public int Type { get; set; }
        public string SearchKeyword { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }

    }

}