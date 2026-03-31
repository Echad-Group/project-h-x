using ProjectHX.Models.Data;
using System.Net;
using System.Text;

namespace TaxiFinder.Helpers
{
	public class HttpHelper
	{
		public HttpHelper()
		{

		}

		public static async Task<bool> CheckConnectionAsync(string url, HttpClient client, string token = "")
		{
			if (url.Contains("ngrok")) client.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "69420");
			if (!string.IsNullOrEmpty(token)) client.DefaultRequestHeaders.Authorization = new("Bearer", token);
			try
			{
				var result = await client.GetAsync(url);
				if (result.StatusCode == HttpStatusCode.OK)
				{
					return true;
				}
				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}

		/*public static async Task<bool> CheckInternetConnectionAsync()
		{
			if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
			{
				return true;
			}
			return false;
		}

		public static bool CheckInternetConnection()
		{
			if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
			{
				return true;
			}
			return false;
		}*/

			public static async Task<RequestResponse> PostFormFileAsync(string url, MultipartFormDataContent formDataContent, HttpClient client, string token = "")
		{
			if (url.Contains("ngrok")) client.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "69420");
			if (!string.IsNullOrEmpty(token)) client.DefaultRequestHeaders.Authorization = new("Bearer", token);
			try
			{
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
				var response = await client.PostAsync(url, formDataContent);
				if (response.StatusCode == HttpStatusCode.OK)
				{
					var res = new RequestResponse(true, "Success", response.ReasonPhrase!)
					{
						Data = await response.Content.ReadAsStringAsync()
					};
					return res;
				}
				else
				{
					var res = new RequestResponse(false, $"There is an issues with the server. Please try again and if this persist then report the error.");
					return res;
				}
			}
			catch (Exception ex)
			{
#if DEBUG
				return new(false, "Unable to connect to server:" + ex.Message);
#else
										if (ex.Message.Contains("System.Net.WebException: 'SSL handshake aborted:") || ex.Message.Contains("System.Net.Http.HttpRequestException: 'The SSL connection could not be established"))
										{
											return new(false, "It seems you have no data bundle active.");
										}
										else if (ex.Message.Contains("System.Net.WebException: 'Unable to resolve host") || ex.Message.Contains("System.Net.Http.HttpRequestException: 'No such host is known"))
										{
											return new(false, "You have no Internet connection");
										}
										else
										{
											return new(false, "Unable to connect to server");
										}
#endif
			}
		}

		public static async Task<RequestResponse> PutAsync(string url, string content, HttpClient client, string token = "")
		{
			if (url.Contains("ngrok")) client.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "69420");
			if (!string.IsNullOrEmpty(token)) client.DefaultRequestHeaders.Authorization = new("Bearer", token);
			try
			{
				var response = await client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));
				if (response.StatusCode == HttpStatusCode.OK)
				{
					var res = new RequestResponse(true, "Success")
					{
						Data = await response.Content.ReadAsStringAsync()
					};
					return res;
				}
				else
				{
					var res = new RequestResponse(false, $"There is an issues with the server. Please try again and if this persist then report the error.");
					return res;
				}
			}
			catch (Exception ex)
			{

#if DEBUG
				return new(false, "Unable to connect to server:" + ex.Message);
#else
				if (ex.Message.Contains("System.Net.WebException: 'SSL handshake aborted:") || ex.Message.Contains("System.Net.Http.HttpRequestException: 'The SSL connection could not be established"))
				{
					return new(false, "It seems you have no data bundle active.");
				}
				else if (ex.Message.Contains("System.Net.WebException: 'Unable to resolve host") || ex.Message.Contains("System.Net.Http.HttpRequestException: 'No such host is known"))
				{
					return new(false, "You have no Internet connection");
				}
				else
				{
					return new(false, "Unable to connect to server");
				}
#endif
				/*if (ex.Message.Contains("System.Net.WebException: 'SSL handshake aborted:") || ex.Message.Contains("System.Net.Http.HttpRequestException: 'The SSL connection could not be established"))
				{
					return new(false, "It seems you have no data bundle active.");
				}
				else if (ex.Message.Contains("System.Net.WebException: 'Unable to resolve host") || ex.Message.Contains("System.Net.Http.HttpRequestException: 'No such host is known"))
				{
					return new(false, "You have no Internet connection");
				}
				else
				{
#if DEBUG
					return new(false, "Unable to connect to server:" + ex.Message);
#else
					return new(false, "Unable to connect to server");
#endif
			}*/
			}
		}

		public static async Task<RequestResponse> PostAsync(string url, string content, HttpClient client, string token = "")
		{
			if (url.Contains("ngrok")) client.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "69420");
			if (!string.IsNullOrEmpty(token)) client.DefaultRequestHeaders.Authorization = new("Bearer", token);
			try
			{
				var response = await client.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));
				if (response.StatusCode == HttpStatusCode.OK)
				{
					var res = new RequestResponse(true, "Success")
					{
						Data = await response.Content.ReadAsStringAsync()
					};
					return res;
				}
				else
				{
					var res = new RequestResponse(false, $"There is an issues with the server. Please try again and if this persist then report the error.");
					return res;
				}
			}
			catch (Exception ex)
			{

#if DEBUG
				return new(false, "Unable to connect to server:" + ex.Message);
#else
				if (ex.Message.Contains("System.Net.WebException: 'SSL handshake aborted:") || ex.Message.Contains("System.Net.Http.HttpRequestException: 'The SSL connection could not be established"))
				{
					return new(false, "It seems you have no data bundle active.");
				}
				else if (ex.Message.Contains("System.Net.WebException: 'Unable to resolve host") || ex.Message.Contains("System.Net.Http.HttpRequestException: 'No such host is known"))
				{
					return new(false, "You have no Internet connection");
				}
				else
				{
					return new(false, "Unable to connect to server");
				}
#endif
				/*if (ex.Message.Contains("System.Net.WebException: 'SSL handshake aborted:") || ex.Message.Contains("System.Net.Http.HttpRequestException: 'The SSL connection could not be established"))
				{
					return new(false, "It seems you have no data bundle active.");
				}
				else if (ex.Message.Contains("System.Net.WebException: 'Unable to resolve host") || ex.Message.Contains("System.Net.Http.HttpRequestException: 'No such host is known"))
				{
					return new(false, "You have no Internet connection");
				}
				else
				{
#if DEBUG
					return new(false, "Unable to connect to server:" + ex.Message);
#else
					return new(false, "Unable to connect to server");
#endif
			}*/
			}
		}

		public static async Task<RequestResponse> GetContentLengthAsync(string url, HttpClient client, string token = "")
		{
			if (url.Contains("ngrok")) client.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "69420");
			if (!string.IsNullOrEmpty(token)) client.DefaultRequestHeaders.Authorization = new("Bearer", token);
			client.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "69420");
			try
			{
				var response = await client.GetAsync(url);
				if (response.StatusCode == HttpStatusCode.OK)
				{
					var res = new RequestResponse(true, "Success")
					{
						Data = response.Content.Headers.ContentLength.ToString()!
					};
					return res;
				}
				else
				{
					var res = new RequestResponse(false, $"There is an issues with the server. Please try again and if this persist then report the error.");
					return res;
				}
			}
			catch (Exception ex)
			{
#if DEBUG
				return new(false, "Unable to connect to server:" + ex.Message);
#else
				if (ex.Message.Contains("System.Net.WebException: 'SSL handshake aborted:") || ex.Message.Contains("System.Net.Http.HttpRequestException: 'The SSL connection could not be established"))
				{
					return new(false, "It seems you have no data bundle active.");
				}
				else if (ex.Message.Contains("System.Net.WebException: 'Unable to resolve host") || ex.Message.Contains("System.Net.Http.HttpRequestException: 'No such host is known"))
				{
					return new(false, "You have no Internet connection");
				}
				else
				{
					return new(false, "Unable to connect to server");
				}
#endif
			}
		}

		public static async Task<RequestResponse> DeleteAsync(string url, HttpClient client, string token = "")
		{
			if (url.Contains("ngrok")) client.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "69420");
			if (!string.IsNullOrEmpty(token)) client.DefaultRequestHeaders.Authorization = new("Bearer", token);
			client.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "69420");
			try
			{
				var response = await client.DeleteAsync(url);
				if (response.StatusCode == HttpStatusCode.OK)
				{
					var res = new RequestResponse(true, "Success")
					{
						Data = await response.Content.ReadAsStringAsync()
					};
					return res;
				}
				else
				{
					var res = new RequestResponse(false, $"There is an issues with the server. Please try again and if this persist then report the error.");
					return res;
				}
			}
			catch (Exception ex)
			{
#if DEBUG
				return new(false, "Unable to connect to server:" + ex.Message);
#else
				if (ex.Message.Contains("System.Net.WebException: 'SSL handshake aborted:") || ex.Message.Contains("System.Net.Http.HttpRequestException: 'The SSL connection could not be established"))
				{
					return new(false, "It seems you have no data bundle active.");
				}
				else if (ex.Message.Contains("System.Net.WebException: 'Unable to resolve host") || ex.Message.Contains("System.Net.Http.HttpRequestException: 'No such host is known"))
				{
					return new(false, "You have no Internet connection");
				}
				else
				{
					return new(false, "Unable to connect to server");
				}
#endif
			}
		}

		public static async Task<RequestResponse> GetAsync(string url, HttpClient client, string token = "")
		{
			if (url.Contains("ngrok")) client.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "69420");
			if (!string.IsNullOrEmpty(token)) client.DefaultRequestHeaders.Authorization = new("Bearer", token);
			client.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "69420");
			try
			{
				var response = await client.GetAsync(url);
				if (response.StatusCode == HttpStatusCode.OK)
				{
					var res = new RequestResponse(true, "Success")
					{
						Data = await response.Content.ReadAsStringAsync()
					};
					return res;
				}
				else
				{
					var res = new RequestResponse(false, $"There is an issues with the server. Please try again and if this persist then report the error.");
					return res;
				}
			}
			catch (Exception ex)
			{
#if DEBUG
				return new(false, "Unable to connect to server:" + ex.Message);
#else
				if (ex.Message.Contains("System.Net.WebException: 'SSL handshake aborted:") || ex.Message.Contains("System.Net.Http.HttpRequestException: 'The SSL connection could not be established"))
				{
					return new(false, "It seems you have no data bundle active.");
				}
				else if (ex.Message.Contains("System.Net.WebException: 'Unable to resolve host") || ex.Message.Contains("System.Net.Http.HttpRequestException: 'No such host is known"))
				{
					return new(false, "You have no Internet connection");
				}
				else
				{
					return new(false, "Unable to connect to server");
				}
#endif
			}
		}
	}
}
