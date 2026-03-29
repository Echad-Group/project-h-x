
namespace ThirdPartyServices.Interfaces
{
	public interface IWhatsAppService
	{
		ResponseModel SendOTP(string phoneNumber, string code);
	}
}
