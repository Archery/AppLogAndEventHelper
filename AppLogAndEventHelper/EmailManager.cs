using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace Mew {
    public class EmailManager
    {
        static EmailManager()
        {
            defaultSmtpClient_ = new SmtpClient("mail.csd.ua", 25)
            {
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(@"e_rudakova@csd.ua", @"7*3DedeP18")
            };
            
            defaultSender_ = new MailAddress("e_rudakova@csd.ua", "CaRu");
        }

        private static readonly SmtpClient defaultSmtpClient_;
        private static readonly MailAddress defaultSender_;

        public static void CreateMessageWithAttachment(string server)
        {
            // Specify the file to be attached and sent.
            // This example assumes that a file named Data.xls exists in the
            // current working directory.
            const string file = "data.xls";
            // Create a message and set up the recipients.
            var message = new MailMessage(
                "jane@contoso.com",
                "ben@contoso.com",
                "Quarterly data report.",
                "See the attached spreadsheet.");

            // Create  the file attachment for this e-mail message.
            var data = new Attachment(file, MediaTypeNames.Application.Octet);
            // Add time stamp information for the file.
            var disposition = data.ContentDisposition;
            disposition.CreationDate = System.IO.File.GetCreationTime(file);
            disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
            disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
            // Add the file attachment to this e-mail message.
            message.Attachments.Add(data);

            //Send the message.
            var client = new SmtpClient(server) {Credentials = CredentialCache.DefaultNetworkCredentials};
            // Add credentials if the SMTP server requires them.

            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateMessageWithAttachment(): {0}",
                    ex.ToString());
            }
            
            data.Dispose();
        }

        public static void SendEmail(string receivers, string subject = "", string body = "")
        {
            var message = new MailMessage(defaultSender_.Address, receivers)
            {
                From = defaultSender_,
                //To = new MailAddress(reciever),
                Subject = subject,
                Body = body
            };

            defaultSmtpClient_.Send(message);
        }

        public static void SendFile(string receivers, FileInfo fi,  string subject = "", string body = "")
        {
            var message = new MailMessage(defaultSender_.Address, receivers)
            {
                From = defaultSender_,
                //To = new MailAddress(reciever),
                Subject = subject,
                Body = body
            };

            var attachment = new Attachment(fi.FullName);
            message.Attachments.Add(attachment);
            defaultSmtpClient_.Send(message);
        }
    }
}
