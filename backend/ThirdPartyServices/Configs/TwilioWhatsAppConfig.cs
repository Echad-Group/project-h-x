namespace ThirdPartyServices.Configs
{
	public class TwilioWhatsAppConfig
	{

		public TwilioWhatsAppConfig()
		{

		}
		public TwilioWhatsAppConfig(string accountSid, string authToken, string sender)
		{
			AccountSid = accountSid;
			AuthToken = authToken;
			Sender = sender;
		}

		public string AccountSid { get; set; } = string.Empty;
		public string AuthToken { get; set; } = string.Empty;
		public string Sender { get; set; } = string.Empty;
	}
}
