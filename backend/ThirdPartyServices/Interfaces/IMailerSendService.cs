using MailerSendNetCore.Emails.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThirdPartyServices.Interfaces
{
	public interface IMailerSendService
	{
		Task<string> SendEmail(string templateId, string senderName, string senderEmail, string[] to, string subject, MailerSendEmailAttachment[] attachments, IDictionary<string, string>? variables, CancellationToken cancellationToken = default);
	}
}
