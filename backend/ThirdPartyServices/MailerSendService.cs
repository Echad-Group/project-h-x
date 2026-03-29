using MailerSendNetCore.Common.Interfaces;
using MailerSendNetCore.Emails.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdPartyServices.Interfaces;
using System.Threading;

namespace ThirdPartyServices
{
	public class MailerSendService : IMailerSendService
	{
		private readonly IMailerSendEmailClient _mailerSendEmailClient;
		private readonly string _logFilePath;

		public MailerSendService(IMailerSendEmailClient mailerSendEmailClient)
		{
			_mailerSendEmailClient = mailerSendEmailClient;
			_logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "MailerSendErrors.log");
		}

		public async Task<string> SendEmail(string templateId, string senderName, string senderEmail, string[] to, string subject, MailerSendEmailAttachment[] attachments, IDictionary<string, string>? variables, CancellationToken cancellationToken = default)
		{
			var parameters = new MailerSendEmailParameters();
			parameters
				.WithTemplateId(templateId)
				.WithFrom(senderEmail, senderName)
				.WithTo(to)
				.WithAttachment(attachments)
				.WithSubject(subject);

			if (variables is { Count: > 0 })
			{
				foreach (var recipient in to)
				{
					parameters.WithPersonalization(recipient, variables);
				}
			}

			var response = await _mailerSendEmailClient.SendEmailAsync(parameters, cancellationToken);
			if (response is { Errors.Count: > 0 })
			{
				LogError(response.Message!, senderEmail);
			}

			return response.MessageId!;
		}

		private void LogError(string errorMessage, string sender)
		{
			try
			{
				using (StreamWriter writer = new StreamWriter(_logFilePath, true))
				{
					writer.WriteLine($"{DateTime.Now}: {sender} => {errorMessage}");
				}
			}
			catch (Exception ex)
			{
				// Handle any exceptions that might occur while logging
				Console.WriteLine($"Failed to log error: {ex.Message}");
			}
		}
	}
}