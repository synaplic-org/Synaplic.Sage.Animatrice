using HtmlAgilityPack;

namespace Uni.Scan.Infrastructure.Services.Email
{
    public interface ITemplateService
    {
        string MailConfirmationTemplate(string verificationUri, string userName);
        string PasswordResetTemplate(string passwordResetURL, string userName);
    }
    public class TemplateService : ITemplateService
    {
        public string MailConfirmationTemplate(string verificationUri, string userName)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.Load(@".\Files\Templates\email-confirmation.html");
            doc.DocumentNode.SelectSingleNode("//*[@class='verificationUri']").SetAttributeValue("href", verificationUri);
            doc.DocumentNode.SelectSingleNode("//*[@class='logo']").SetAttributeValue("src", "https://ste.ma/oc-content/uploads/1388/188.jpg");
            doc.DocumentNode.SelectSingleNode("//*[@class='userName']").InnerHtml = userName;
            var s = doc.DocumentNode.OuterHtml;
            return s;
        }

        public string PasswordResetTemplate(string passwordResetURL, string userName)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.Load(@".\Files\Templates\email-confirmation.html");
            doc.DocumentNode.SelectSingleNode("//*[@class='passwordResetURL']").SetAttributeValue("href", passwordResetURL);
            doc.DocumentNode.SelectSingleNode("//*[@class='passwordResetShowURL']").InnerHtml = passwordResetURL;
            doc.DocumentNode.SelectSingleNode("//*[@class='logo']").SetAttributeValue("src", "https://ste.ma/oc-content/uploads/1388/188.jpg");
            doc.DocumentNode.SelectSingleNode("//*[@class='userName']").InnerHtml = userName;
            var s = doc.DocumentNode.OuterHtml;
            return s;
        }

    }
}
