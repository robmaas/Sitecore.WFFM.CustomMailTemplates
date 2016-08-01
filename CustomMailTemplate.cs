namespace Estate.Framework.WFFM.Pipelines.ProcessMessage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using Sitecore.WFFM.Abstractions.Mail;
    using Sitecore.WFFM.Abstractions.Actions;
    using System.IO;

    public class CustomMailTemplate
    {
        private String MailTemplatePath = Sitecore.Configuration.Settings.GetSetting("Estate.WFFM.CustomMailTemplates.TemplatePath", "~/Content/{0}/MailTemplates/WFFM.html");
        private String Token = Sitecore.Configuration.Settings.GetSetting("Estate.WFFM.CustomMailTemplates.Token", "$$CONTENT$$");

        public void Process(ProcessMessageArgs args)
        {
            var site = Sitecore.Context.GetSiteName();
            if(!string.IsNullOrEmpty(site))
            {
                var mailTemplatePath = HttpContext.Current.Server.MapPath(String.Format(MailTemplatePath, site));
                
                if (File.Exists(mailTemplatePath))
                {
                    var template = File.ReadAllText(mailTemplatePath) ?? string.Empty;

                    if (template.Contains(Token))
                    {
                        var body = template.Replace(Token, args.Mail.ToString());

                        args.IsBodyHtml = true;
                        args.Mail.Clear();
                        args.Mail.Append(body);
                    }
                }
            }
        }
    }
}
