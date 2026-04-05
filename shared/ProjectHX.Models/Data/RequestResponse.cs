
namespace ProjectHX.Models.Data
{
	public class RequestResponse: Response
	{
		public RequestResponse()
		{
				
		}
		public RequestResponse(bool success, string message)
		{
			Success = success;
			Message = message;
		}
		public RequestResponse(bool success, string message, string data)
		{
			Success = success;
			Message = message;
			Data = data;
		}
		public string Data { get; set; } = string.Empty;
	}
}
