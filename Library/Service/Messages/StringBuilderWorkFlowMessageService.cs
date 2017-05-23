using System;
using System.Configuration;
using System.IO;
using System.Text;
using Entities.Domain;
using Entities.Domain.Users;
using Service.Departments;
using Service.Users;

namespace Service.Messages
{
    public class StringBuilderWorkFlowMessageService : IWorkFlowMessageService
    {

        public string fromName = ConfigurationManager.AppSettings["EmailFromName"];
        public string from = ConfigurationManager.AppSettings["MailFrom"];
        public string baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        private readonly IUserService _userService;
        //private readonly IAttendanceService _attendanceService;
        public StringBuilderWorkFlowMessageService(IUserService userService//, 
            //IAttendanceService attendanceService
            )
        {
            _userService = userService;
            //_attendanceService = attendanceService;
        }

        public QueuedEmail SendReportToUser(User user, string department, string fromDate, string toDate)
        {
            var html = new StringBuilder();
            //html.AppendFormat("<p>Hello {0}!</p>", user.UserName);
            //html.Append("<div><span>Department: </span>" + department + " </div>" +
            //            "<div><span>Report from : </span> " + fromDate + "<span>  to : </span> " + toDate + " </div>"
            //            );
            // Get Html file 

            var folderpath = AppDomain.CurrentDomain.BaseDirectory + "HtmlTemplateEmail";
            var filePath = Path.Combine(folderpath, "SendMailIReport.html");
            string htmlContent = File.ReadAllText(filePath);
            htmlContent.Replace("#UserName#", user.Username);
            htmlContent.Replace("#Department#", department);
            htmlContent.Replace("#FromDate#", fromDate);
            htmlContent.Replace("#ToDate#", toDate);

            // Append to html body send mail
            html.Append(htmlContent);
            var queuedEmail = new QueuedEmail
            {
                Body = html.ToString(),
                Bcc = "",
                Cc = "",
                CreatedOnUtc = DateTime.Now,
                From = from,
                FromName = fromName,
                Priority = 5,
                SentOnUtc = null,
                SentTries = 0,
                Subject = "Sample Report Email",
                To = user.Email,
                ToName = user.Username
            };

           

            return queuedEmail;
        }
        public QueuedEmail SendWarningMark(string email, int id, string createDate, int valueMark,int severityValue)
        {
            var html = new StringBuilder();
            var folderpath = AppDomain.CurrentDomain.BaseDirectory + "HtmlTemplateEmail";
            var filePath = Path.Combine(folderpath, "SendMailWarningMark.html");
            string htmlContent = File.ReadAllText(filePath);

            htmlContent = htmlContent.Replace("#Id#", "" + id);
            htmlContent = htmlContent.Replace("#CreateDate#", createDate);
            if (valueMark>0)
            {
                htmlContent = htmlContent.Replace("#ValueMark#", "" + valueMark);
                htmlContent = htmlContent.Replace("<div id='divMark' style='display:none'>", "<div id='divMark' style='display:block'>");
            }
            if(severityValue>0)
            {
                htmlContent = htmlContent.Replace("#SeverityValue#", "" + severityValue);
                htmlContent = htmlContent.Replace("<div id='divSeverity' style='display:none'>", "<div id='divSeverity' style='display:block'>");
            }

            // Append to html body send mail
            html.Append(htmlContent);
            var queuedEmail = new QueuedEmail
            {
                Body = html.ToString(),
                Bcc = "",
                Cc = "",
                CreatedOnUtc = DateTime.Now,
                From = from,
                FromName = fromName,
                Priority = 5,
                SentOnUtc = null,
                SentTries = 0,
                Subject = "Sample Warning Mark Email",
                To = email,
                ToName = email
            };
            return queuedEmail;
        }

        public QueuedEmail SendIssusToUser(User user,Issue issues)
        {
            var html = new StringBuilder();
            //html.AppendFormat("<p>Hello {0}!</p>", user.UserName);
            //html.Append("<div><h2>Send to Assigner: " + user.UserName + "</h2> </div>" +
            //            "<div><span>Issues: </span>" + issues.Content + " </div>" +
            //            "<div><span>Status: </span> " + issues.Status + "</div>" +
            //            "<div><span>Create Date: </span> " + issues.CreatedDate.ToShortDateString() + "</div>" +
            //            "<div><span>Update Date: </span> " + issues.UpdatedDate.ToShortDateString() + "</div>" +
            //            "<div><span>Owner: </span>" + issues.CreatedUser.UserName + " </div>"
            //            );
           

            // Get Html file 
            var folderpath = AppDomain.CurrentDomain.BaseDirectory + "HtmlTemplateEmail";
            var filePath = Path.Combine(folderpath, "SendMailIIssuesToUser.html");

            string htmlContent = File.ReadAllText(filePath);

            htmlContent.Replace("#UserName#", user.Username);
            htmlContent.Replace("#Content#", issues.Content);
            htmlContent.Replace("#Status#", issues.IssueStatus.ToString());
            htmlContent.Replace("#CreatedDate#", issues.CreatedDate.ToShortDateString());
            htmlContent.Replace("#UpdatedDate#", issues.UpdatedDate.ToShortDateString());
            htmlContent.Replace("#Owner#", issues.User.Username);

            // Append to html body send mail
            html.Append(htmlContent);


            var queuedEmail = new QueuedEmail
            {
                Body = html.ToString(),
                Bcc = "",
                Cc = "",
                CreatedOnUtc = DateTime.Now,
                From = from,
                FromName = fromName,
                Priority = 5,
                SentOnUtc = null,
                SentTries = 0,
                Subject = "P&G Meeting - Assigned Issues",
                To = user.Email,
                ToName = user.Username
            };

            return queuedEmail;
        }

        public QueuedEmail SendIssusToOwner(User user,Issue issues,string userUpdate)
        {
            var html = new StringBuilder();
            //html.AppendFormat("<p>Hello {0}!</p>", user.UserName);
            //html.Append("<div><h2>Send to Owner: " + user.UserName + "</h2> </div>" +
            //            "<div><span>Issues: </span>" + issues.Content + " </div>" +
            //            "<div><span>New Status: </span> " + issues.Status + "</div>" +
            //            "<div><span>Update Date: </span> " + issues.UpdatedDate.ToShortDateString() + "</div>" +
            //            "<div><span>User Update: </span>" + userUpdate + " </div>"
            //            );

            // Get Html file 
            var folderpath = AppDomain.CurrentDomain.BaseDirectory + "HtmlTemplateEmail";
            var filePath = Path.Combine(folderpath, "SendMailIIssuesToOwner.html");
           
            string htmlContent = File.ReadAllText(filePath);

            htmlContent = htmlContent.Replace("#Owner#", user.Username)
                .Replace("#Content#", issues.Content)
                .Replace("#Status#", issues.IssueStatus.ToString())
                .Replace("#UpdatedDate#", issues.UpdatedDate.ToShortDateString())
                .Replace("#UserName#", userUpdate);

            // Append to html body send mail
            html.Append(htmlContent);


            var queuedEmail = new QueuedEmail
            {
                Body = html.ToString(),
                Bcc = "",
                Cc = "",
                CreatedOnUtc = DateTime.Now,
                From = from,
                FromName = fromName,
                Priority = 5,
                SentOnUtc = null,
                SentTries = 0,
                Subject = "P&G Meeting - Changed Issues Status ",
                To = user.Email,
                ToName = user.Username
            };



            return queuedEmail;
        }

       

        //public QueuedEmail SendAttendToOwner(User user, AttendancePerDay attendancePerDay)
        //{
        //    var userCreate = new User();
        //    if (attendancePerDay.CreatedUserId != 0)
        //        userCreate = _userService.GetUserByIdAsync(attendancePerDay.CreatedUserId).Result;
        //        var userCreateName = userCreate == null ? "" : userCreate.Username;
            
        //    var html = new StringBuilder();
        //    //html.AppendFormat("<p>Hello {0}!</p>", user.UserName);
        //    //html.Append("<div><h2>Send to Owner: " + user.UserName + "</h2> </div>" +
        //    //            "<div><span>Attendance at: </span>" + attendancePerDay.CreatedDate.ToShortDateString() + " </div>" +
        //    //            "<div><span>User Create: </span>" + userCreateName + " </div>"
        //    //            );


        //    // Get list user addtendance
        //    var listUserNameInAttendance =_attendanceService.GetUsernameInAttendance(attendancePerDay.Id);
        //    var listStringUserNameInAttendance = String.Join(";", listUserNameInAttendance);

        //    // Get list user not addtendance
        //    var listUserNameNotInAttendance = _attendanceService.GetUsernameNotInAttendance(attendancePerDay.Id);
        //    var listStringUserNameNotInAttendance = String.Join(";", listUserNameNotInAttendance);

        //    var folderpath = AppDomain.CurrentDomain.BaseDirectory + "HtmlTemplateEmail";
        //    var filePath = Path.Combine(folderpath, "SendMailIAttendance.html");


        //    string htmlContent = File.ReadAllText(filePath);
        //    htmlContent.Replace("#UserName#", user.Username);
        //    htmlContent.Replace("#CreatedDate#", attendancePerDay.CreatedDate.ToShortDateString());
        //    htmlContent.Replace("#Owner#", userCreateName);
        //    htmlContent.Replace("#ListUserNameInAttendance#", listStringUserNameInAttendance);
        //    htmlContent.Replace("#ListUserNameNotInAttendance#", listStringUserNameNotInAttendance);
        
        //    var queuedEmail = new QueuedEmail
        //    {
        //        Body = html.ToString(),
        //        Bcc = "",
        //        Cc = "",
        //        CreatedOnUtc = DateTime.Now,
        //        From = from,
        //        FromName = fromName,
        //        Priority = 5,
        //        SentOnUtc = null,
        //        SentTries = 0,
        //        Subject = "Attendance",
        //        To = user.Email,
        //        ToName = user.Username
        //    };



        //    return queuedEmail;
        //}

    }
}
