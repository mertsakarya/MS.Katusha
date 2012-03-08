
using System;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Runtime.Caching;
using RazorEngine;

namespace MS.Katusha.Mailer
{
    public static class Mailer
    {
        public static void SendMail<T>(string to, string subject, string templateName, T model)
        {


            string @from = ConfigurationManager.AppSettings["Mail_From"] ?? "mertsakarya@gmail.com";
            string password = ConfigurationManager.AppSettings["Mail_Password"] ?? "Sakarya69&";
            string host = ConfigurationManager.AppSettings["Mail_Host"] ?? "smtp.gmail.com";
            int port; if(!int.TryParse(ConfigurationManager.AppSettings["Mail_Port"], out port)) port = 587;
            bool enableSsl;  if(!bool.TryParse(ConfigurationManager.AppSettings["Mail_From"], out enableSsl)) enableSsl = true;
            string displayName = ConfigurationManager.AppSettings["Mail_DisplayName"] ?? "Mert Sakarya";
            string templateFolder = ConfigurationManager.AppSettings["Template_Folder"] ?? "~\\___MailTemplates\\";
            string templateFileName;
            string templateText = null;
            string result = "";
            if (!templateName.EndsWith(".cshtml")) templateText = templateName;
            if(templateText != null)
            {
                result = Razor.Parse(templateName, model);
            } else
            {
                ObjectCache cache = MemoryCache.Default;
                if (!cache.Contains(templateName))
                {
                    CreateCache<T>(templateName, cache, templateFolder);
                } 
                else
                {
                    var cachedCreation = (DateTime) cache.Get(templateName);
                    var fileName = templateFolder + templateName;
                    var creation = File.GetLastWriteTime(fileName);
                    if(cachedCreation != creation)
                        CreateCache<T>(templateName, cache, templateFolder);
                }
                result = Razor.Run(templateName, model);
            }
            //string template = "Some really complex template that will take time to parse";

            //Razor.Compile(template, "complex");
            //Razor.Run("complex");

            

            using (var mail = new MailMessage())
            {
                mail.To.Add(to);
                mail.From = new MailAddress(@from, displayName, System.Text.Encoding.UTF8);
                mail.Subject = subject;
                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                mail.Body = result;
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;
                var client = new SmtpClient
                        {
                            Credentials = new System.Net.NetworkCredential(@from, password),
                            Port = port,
                            Host = host,
                            EnableSsl = enableSsl
                        };

                try
                {
                    client.Send(mail);
                }
                catch (Exception ex)
                {
                    //Exception ex2 = ex;
                    //string errorMessage = string.Empty;
                    //while (ex2 != null)
                    //{
                    //    errorMessage += ex2.ToString();
                    //    ex2 = ex2.InnerException;
                    //}
                    //HttpContext.Current.Response.Write(errorMessage);
                } // end try 
            }
        }

        private static void CreateCache<T>(string templateName, ObjectCache cache, string templateFolder)
        {
            string templateText;
            var fileName = templateFolder + templateName;
            var creation = File.GetLastWriteTime(fileName);
            using (var reader = new StreamReader(fileName))
                templateText = reader.ReadToEnd();
            cache.Add(templateName, creation, DateTime.MaxValue);
            Razor.Compile<T>(templateText, templateName);
        }
    }
}
