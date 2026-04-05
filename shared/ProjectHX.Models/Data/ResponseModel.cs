namespace ProjectHX.Models.Data
{
	public class ResponseModel
	{
		public ResponseModel()
		{

		}
		public ResponseModel(bool s, string msg)
		{
			Success = s;
			Message = msg;
		}
		public bool Success { get; set; }
		public string Message { get; set; } = string.Empty;
	}
}
