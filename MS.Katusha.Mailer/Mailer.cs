﻿
using System;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Runtime.Caching;
using System.Web;
using RazorEngine;

namespace MS.Katusha.Mailer
{
    public static class Mailer
    {
        public static string SendMail<T>(string to, string subject, string templateName, string templateFolder, T model)
        {
            var @from = ConfigurationManager.AppSettings["Mail_From"] ?? "mskatusha.info@gmail.com";

            string templateText = null;
            string result;
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

            using (var mail = new MailMessage())
            {
                mail.To.Add(to);
                mail.From = new MailAddress(@from, "", System.Text.Encoding.UTF8);
                mail.Subject = subject;
                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                mail.Body = result;
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.IsBodyHtml = true;
//               mail.Priority = MailPriority.High;
                using (var client = new SmtpClient()) {
                    try {
                        client.Send(mail);
                    } catch {
                    } // end try 
                }
            }
            return result;
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
