namespace ProjectHX.Models.Data
{
	public class Response
	{
		public Response()
		{
				
		}
		public Response(bool success, string message)
		{
			Success = success;
			Message = message;
		}
		public bool Success { get; set; }
		public string Message { get; set; } = string.Empty;
	}
}
