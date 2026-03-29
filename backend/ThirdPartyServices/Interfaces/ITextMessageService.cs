using Twilio.Rest.Api.V2010.Account;

namespace ThirdPartyServices.Interfaces
{
	public interface ITextMessageService
	{
		MessageResource SendTextMessage(string phoneNumber, string message);
	}
}