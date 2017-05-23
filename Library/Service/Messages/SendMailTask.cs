using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Service.Tasks;

namespace Service.Messages
{
    public class SendMailTask : ITask
    {
        private int SentTry = 10;
        private readonly IQueuedEmailService _queuedEmailService;

        public SendMailTask(IQueuedEmailService queuedEmailService)
        {
            _queuedEmailService = queuedEmailService;
        }


        public void Execute()
        {

            var listEmail = _queuedEmailService.GetAllNotAsync();
            var listEmailNeedToSend = listEmail.Where(p => p.SentOnUtc == null && p.SentTries < SentTry);
            foreach (var queueEmail in listEmailNeedToSend)
            {

                var message = new MailMessage();
                message.From = new MailAddress(queueEmail.From, queueEmail.FromName);
                message.To.Add(new MailAddress(queueEmail.To, queueEmail.ToName));

                message.Subject = queueEmail.Subject;
                message.Body = "<div style='font-family:Arial; font-size:12px;'>" + queueEmail.Body + "</div>";
                message.IsBodyHtml = true;
                //message.Attachments.Add(new Attachment(Server.MapPath(strFileName)));
                foreach (var attach in queueEmail.QueuedEmailAttachments)
                {
                    var folderForSaveFile = ConfigurationManager.AppSettings["excelforderpath"].ToString();
                    var folderpath = AppDomain.CurrentDomain.BaseDirectory + folderForSaveFile;
                    var fileName = Path.Combine(folderpath, attach.FilePath);
                    var attachment1 = new Attachment(fileName);
                    message.Attachments.Add(attachment1);
                }
                
          


                try
                {
                    var smtp = ConfigurationManager.AppSettings["Smtp"];
                    var port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
                    var mailUser = ConfigurationManager.AppSettings["MailUser"];
                    var mailPassword = ConfigurationManager.AppSettings["MailPassword"];
                    var enableSsl = bool.Parse(ConfigurationManager.AppSettings["EnableSsl"]);

                    using (var smtpClient = new SmtpClient())
                    {
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Host = smtp;
                        smtpClient.Port = port;
                        smtpClient.EnableSsl = enableSsl;
                        smtpClient.Credentials = new NetworkCredential(mailUser, mailPassword);
                        
                        smtpClient.Send(message);
                    }
                    queueEmail.SentOnUtc = DateTime.Now;
                    _queuedEmailService.UpdateNotAsync(queueEmail);

                }
                catch (Exception ex)
                {
                    queueEmail.SentTries += 1;
                    _queuedEmailService.UpdateNotAsync(queueEmail);
                }    
            }
        }
    }
}
