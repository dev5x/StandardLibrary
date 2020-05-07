using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using Outlook = Microsoft.Office.Interop.Outlook;

/* dev5x.com (c) 2020
 * 
 * Email Class
 * Handles general email needs
*/

namespace dev5x.StandardLibrary
{
    public class Email : BaseClass
    {
        private readonly string _smtpServer = string.Empty;
        private readonly string _smtpUser = string.Empty;
        private readonly string _smtpPassword = string.Empty;
        private readonly string _smtpPort = "25";
        private readonly bool   _useCredentials = false;

        public enum MailFormat
        {
            mailHTML,
            mailText
        }

        public Email()
        {
        }

        public Email(string SmtpServer, string SmtpUser, string SmtpPassword)
        {
            try
            {
                // Parse the port number from the server address if present
                int portNumberPos = SmtpServer.IndexOf(":");

                if (portNumberPos > 0)
                {
                    _smtpServer = SmtpServer.Substring(0, portNumberPos);
                    _smtpPort = SmtpServer.Substring(portNumberPos + 1);
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

        public bool SMTPSendMail(string[] MailTO, string[] MailCC, string[] MailBCC, string MailSubject, string MailBody, string[] MailAttachments, MailFormat MessageFormat)
        {
            try
            {
                // Send SMTP mail
                using (MailMessage message = new MailMessage())
                {
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

                    message.From = new MailAddress(_smtpUser);

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
                        message.Body = "<html><body><br />" + MailBody + "</body></html>";
                    }
                    else
                    {
                        message.IsBodyHtml = false;
                        message.BodyEncoding = Encoding.UTF8;
                        message.Body = MailBody;
                    }

                    if (MailAttachments != null && MailAttachments.Length > 0)
                    {
                        foreach (string attachment in MailAttachments)
                        {
                            if (File.Exists(attachment))
                            {
                                message.Attachments.Add(new Attachment(attachment));
                            }
                        }
                    }

                    SmtpClient smtpClient = new SmtpClient(_smtpServer);

                    if (_useCredentials)
                    {
                        smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
                    }

                    smtpClient.Port = Convert.ToInt32(_smtpPort);
                    smtpClient.Send(message);

                    return true;
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage("SMTPSendMail - " + ex.Message);
                return false;
            }
        }

        public void OutlookMail(string[] FilePaths, string MailSubject, string MailBody)
        {
            try
            {
                // Create new Outlook mail message with attachments
                Outlook.Application appOutlook = new Outlook.Application();
                Outlook.MailItem mailItem = (Outlook.MailItem)appOutlook.CreateItem(Outlook.OlItemType.olMailItem);
                string winTemp = Path.GetTempPath();
                string[] tempFiles = new string[0];
                int pos = MailBody.Length;
                int cnt = 0;

                if (FilePaths != null)
                {
                    tempFiles = new string[FilePaths.Length];
                    pos += FilePaths.Length;

                    foreach (string fullPath in FilePaths)
                    {
                        FileInfo fi = new FileInfo(fullPath);

                        if (fi.Exists)
                        {
                            // Copy the file to the temp folder and update its location
                            fi.CopyTo(Path.Combine(winTemp, fi.Name), true);
                            tempFiles[cnt] = Path.Combine(winTemp, fi.Name);

                            // Add attachment to email
                            Outlook.Attachment attachment = mailItem.Attachments.Add(fullPath, Outlook.OlAttachmentType.olByValue, pos, fi.Name);

                            pos--;
                            cnt++;
                        }
                    }
                }

                // Set optional values
                if (MailSubject != null && MailSubject.Length > 0)
                {
                    mailItem.Subject = MailSubject;
                }

                if (MailBody != null && MailBody.Length > 0)
                {
                    mailItem.Body = MailBody;
                }

                // Display message
                mailItem.Display(false);

                // Delete the files from the temp folder
                foreach (string file in tempFiles)
                {
                    FileInfo fi = new FileInfo(file);

                    if (fi.Exists)
                    {
                        // Change attribute to normal on read-only files
                        if (fi.IsReadOnly)
                        {
                            fi.Attributes = FileAttributes.Normal;
                        }

                        // Delete temp file
                        fi.Delete();
                    }
                }

            }
            catch (Exception ex)
            {
                SetErrorMessage("OutlookMail - " + ex.Message);
            }
        }
    }
}