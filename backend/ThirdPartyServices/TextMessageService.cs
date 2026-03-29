using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextFlow;
using ThirdPartyServices.Configs;
using ThirdPartyServices.Interfaces;
using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace ThirdPartyServices
{
	public class TextMessageService: ITextMessageService
	{
		private readonly ILogger<ITextMessageService> _logger;
		private readonly TwilioSMSConfig _twilioSMSConfig;

		public TextMessageService(ILogger<ITextMessageService> logger, TwilioSMSConfig config)
		{
			_logger = logger;
			_twilioSMSConfig = config;
		}

		public MessageResource SendTextMessage(string phoneNumber, string message)
		{
			TwilioClient.Init(_twilioSMSConfig.AccountSid, _twilioSMSConfig.AuthToken);

			var messageOptions = new CreateMessageOptions(new PhoneNumber(phoneNumber));
			messageOptions.From = new PhoneNumber(_twilioSMSConfig.Sender);
			messageOptions.Body = message;


			var messageResource = MessageResource.Create(messageOptions);

			Console.WriteLine($"Sending text message to {phoneNumber} with message: {messageResource.Body}");
			_logger.LogInformation($"Sending text message to {phoneNumber} with message: {messageResource.Body}");

			return messageResource;
		}
	}
}
