namespace Estate.Framework.WFFM.Pipelines.ProcessMessage
{
    using System;
    using System.Web;
    using System.IO;
    using Sitecore.Form.Core.Pipelines.ProcessMessage;
    using HtmlAgilityPack;
    using Sitecore;

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
                        body = Absolutify(body);
                        args.IsBodyHtml = true;
                        args.Mail.Clear();
                        args.Mail.Append(body);
                    }
                }
            }
        }

        private string Absolutify(string body)
        {
            var html = new HtmlDocument();
            html.LoadHtml(body);

            foreach(HtmlNode node in html.DocumentNode.SelectNodes("//img[@src]"))
            {
                string src = node.GetAttributeValue("src", string.Empty);
                Absolutify(src, node, "src");
            }

            foreach (HtmlNode node in html.DocumentNode.SelectNodes("//a[@href]"))
            {
                string href = node.GetAttributeValue("href", string.Empty);
                Absolutify(href, node, "href");
            }

            return html.DocumentNode.OuterHtml;
        }

        private void Absolutify(string href,  HtmlNode node, string attributeName)
        {
            var uri = new Uri(href, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri)
            {

                string url = StringUtil.EnsurePrefix('/', uri.OriginalString);
                UriBuilder builder = new UriBuilder(HttpContext.Current.Request.Url);
                builder.Path = uri.OriginalString;
                uri = builder.Uri;
                node.SetAttributeValue(attributeName, uri.ToString());
            }
        }
    }
}
