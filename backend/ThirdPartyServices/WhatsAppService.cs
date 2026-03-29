using ThirdPartyServices.Configs;
using ThirdPartyServices.Interfaces;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ThirdPartyServices
{
	public class WhatsAppService: IWhatsAppService
	{
		private readonly TwilioWhatsAppConfig twilioWhatsAppConfig;
		public WhatsAppService(TwilioWhatsAppConfig twilioWhatsAppConfig)
		{
			this.twilioWhatsAppConfig = twilioWhatsAppConfig;
		}

		public ResponseModel SendOTP(string phoneNumber, string code)
		{
			try
			{
				TwilioClient.Init(twilioWhatsAppConfig.AccountSid, twilioWhatsAppConfig.AuthToken);

				var messageOptions = new CreateMessageOptions(new PhoneNumber($"whatsapp:{phoneNumber}"))
				{
					From = new PhoneNumber($"whatsapp:{twilioWhatsAppConfig.Sender}"),
					Body = $"Your One-time verification code is {code}"
				};


				var message = MessageResource.Create(messageOptions);
				//Console.WriteLine(message.Body);
				return new(true, message.Body);
			}
			catch (Exception)
			{
				return new(false, "An error occured please try and if it persist, contact support. Thanks");
			}
		}
	}
}