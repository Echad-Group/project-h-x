namespace ProjectHX.Mobile.Contexts
{
	public class AppHttpContext
	{
		private readonly HttpClient Client;

		public AppHttpContext(string baseUrl, string url, HttpClient client)
		{
			ApiUrlBase = baseUrl;
			ApiUrlHost = url;
			Client = client;
		}

		public string Post()
		{
			return "";
		}


		public string ApiUrlBase { get; set; } = string.Empty;
		public string ApiUrlHost { get; set; } = string.Empty;
		//public string BaseUrl { get; set; } = $"{ApiUrlHost}";
	}
}
