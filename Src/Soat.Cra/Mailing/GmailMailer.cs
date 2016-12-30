using Soat.Cra.Credential;
using Soat.Cra.Interfaces;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace Soat.Cra.Mailing
{
    public class GmailMailer : IMailer
    {
        private readonly ITemplater _templater;

        private readonly string _mailFrom;
        private readonly string _mailTo;
        private readonly string _mailSubject;

        public GmailMailer(ITemplater templater)
        {
            _templater = templater;

            _mailFrom = ConfigurationManager.AppSettings["Mailing.From"];
            _mailTo = ConfigurationManager.AppSettings["Mailing.To"];
            _mailSubject = ConfigurationManager.AppSettings["Mailing.Subject"];
        }

        public void Mail(UserAccount account, IEnumerable<string> attachedFiles, int month, int year)
        {
            var fromAddress = new MailAddress(account.Username, _mailFrom);
            var toAddress = new MailAddress(_mailTo, _mailTo);

            var subject = _mailSubject;
            var body = _templater.Template(new Dictionary<string, string>
            {
                { "Month", month.ToString() },
                { "Year", year.ToString() },
                { "Signature", _mailFrom }
            });

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, account.Password)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                foreach (var attachedFile in attachedFiles)
                    message.Attachments.Add(new Attachment(attachedFile));

                smtp.Send(message);
            }
        }
    }
}
