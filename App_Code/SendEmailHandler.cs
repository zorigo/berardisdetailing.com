using System;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;

public class SendEmailHandler : IHttpHandler
{
	#region IHttpHandler Members

	public bool IsReusable
	{
		get { return true; }
	}

	public void ProcessRequest(HttpContext context)
	{
		using (MailMessage message = new MailMessage())
		using (SmtpClient smtp = new SmtpClient())
		{
			string smtpHost = WebConfigurationManager.AppSettings["Smtp:Host"];
			int smtpPort = Convert.ToInt32(WebConfigurationManager.AppSettings["Smtp:Port"]);
			string smtpUsername = WebConfigurationManager.AppSettings["Smtp:Username"];
			string smtpPassword = WebConfigurationManager.AppSettings["Smtp:Password"];
		
			var creds = new NetworkCredential(smtpUsername, smtpPassword);
			var auth = creds.GetCredential(smtpHost, smtpPort, "Basic");
		
			smtp.Host = smtpHost;
			smtp.Port = smtpPort;
			smtp.Credentials = auth;
		
			string contactName = context.Request.Form["contact-name"];
			string contactEmail = context.Request.Form["contact-email"];

			message.To.Add(new MailAddress(WebConfigurationManager.AppSettings["InfoEmail"], "Berardi's Detailing"));
			message.To.Add(new MailAddress(contactEmail, contactName));
			message.From = new MailAddress(WebConfigurationManager.AppSettings["NoReplyEmail"], "Berardi's Detailing No Reply");
			message.ReplyTo = new MailAddress(contactEmail, contactName);

			message.Subject = "Web Site Information Request";
			message.IsBodyHtml = false;
			message.Body = "sent-at:" + Environment.NewLine + DateTime.Now.ToString("F") + Environment.NewLine + Environment.NewLine;
			message.Body += "sent-from:" + Environment.NewLine + "berardisdetailing.com" + Environment.NewLine + Environment.NewLine;

			foreach (string id in context.Request.Form.AllKeys)
				message.Body += id + ":" + Environment.NewLine + context.Request.Form[id] + Environment.NewLine + Environment.NewLine;

			smtp.Send(message);
		}

		// redirect to form
		context.Response.Redirect("/thank-you.html");
	}

	#endregion
}