using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;

/* dev5x.com (c) 2020
 * 
 * SMTP Email Class
 * Handles general email needs
*/

namespace dev5x.StandardLibrary
{
    public class SMTPMail : BaseClass
    {
        private readonly string _smtpServer = string.Empty;
        private readonly string _smtpUser = string.Empty;
        private readonly string _smtpPassword = string.Empty;
        private readonly bool   _useCredentials = false;

        public int Port { get; set; } = 25; 
        public bool UseSSL { get; set; } = false;
        public string DisplayName { get; set; } = string.Empty;

        public enum MailFormat
        {
            mailHTML,
            mailText
        }

        public SMTPMail(string SmtpServer, string SmtpUser, string SmtpPassword)
        {
            try
            {
                // Parse the port number from the server address if present
                int portNumberPos = SmtpServer.IndexOf(":");

                if (portNumberPos > 0)
                {
                    _smtpServer = SmtpServer.Substring(0, portNumberPos);

                    string port = SmtpServer.Substring(portNumberPos + 1);
                    if (Utility.IsNumeric(port))
                    {
                        Port = Convert.ToInt32(port);
                    }
                }
                else
                {
                    _smtpServer = SmtpServer;
                }

                if (SmtpUser.Length > 0)
                {
                    _useCredentials = true;
                    _smtpUser = SmtpUser;

                    Encryption encrypt = new Encryption();
                    _smtpPassword = encrypt.Decrypt(SmtpPassword);
                }
            }
            catch { }
        }

        public bool SendMail(List<string> MailTO, List<string> MailCC, List<string> MailBCC, string MailSubject, string MailBody, List<string> MailAttachments, MailFormat MessageFormat)
        {
            try
            {
                // Send SMTP mail
                using (MailMessage message = new MailMessage())
                {
                    // Get addresses
                    if (MailTO != null)
                    {
                        foreach (string to in MailTO)
                        {
                            if (to != null && to.Trim().Length > 0)
                            {
                                message.To.Add(to);
                            }
                        }
                    }

                    if (MailCC != null)
                    {
                        foreach (string cc in MailCC)
                        {
                            if (cc != null && cc.Trim().Length > 0)
                            {
                                message.CC.Add(cc);
                            }
                        }
                    }

                    if (MailBCC != null)
                    {
                        foreach (string bcc in MailBCC)
                        {
                            if (bcc != null && bcc.Trim().Length > 0)
                            {
                                message.Bcc.Add(bcc);
                            }
                        }
                    }

                    // Prepare message parts
                    message.From = new MailAddress(_smtpUser, DisplayName);

                    if (MailSubject != null)
                    {
                        message.Subject = MailSubject;
                    }

                    if (MailBody == null)
                    {
                        MailBody = string.Empty;
                    }

                    if (MessageFormat == MailFormat.mailHTML)
                    {
                        message.IsBodyHtml = true;
                        message.Body = "<html><body>" + MailBody + "</body></html>";
                    }
                    else
                    {
                        message.IsBodyHtml = false;
                        message.BodyEncoding = Encoding.UTF8;
                        message.Body = MailBody;
                    }

                    if (MailAttachments != null && MailAttachments.Count > 0)
                    {
                        foreach (string attachment in MailAttachments)
                        {
                            if (File.Exists(attachment))
                            {
                                message.Attachments.Add(new Attachment(attachment));
                            }
                        }
                    }

                    SmtpClient smtpClient = new SmtpClient(_smtpServer, Port);

                    if (_useCredentials)
                    {
                        smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
                        smtpClient.EnableSsl = UseSSL;
                    }

                    smtpClient.Send(message);

                    return true;
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage("SendMail - " + ex.Message);
                return false;
            }
        }
    }
}