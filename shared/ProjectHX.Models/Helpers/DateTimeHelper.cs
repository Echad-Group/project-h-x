
namespace TaxiFinder.Helpers
{
	public static class DateTimeHelper
	{
		/*public static async Task<string> GetViewingDeviceTimeZoneViaJSInteropAsync(IJSRuntime JSRuntime)
		{
			return await JSRuntime.InvokeAsync<string>("timezoneHelper.getTimeZone");
		}
		public static async Task<DateTime> GetLocalTime(DateTime localTime, IJSRuntime jSRuntime)
		{
			string timeZone = await GetViewingDeviceTimeZoneViaJSInteropAsync(jSRuntime);
			return GetTimeInLocalTimeZone(localTime, timeZone);
		}*/

		public static DateTime GetTimeInLocalTimeZone(DateTime utcDateTime, string localTimeZone)
		{
			TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(localTimeZone); // or another time zone
			DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);
			return localTime;
		}

		public static DateTime GetTimeInLocalTimeZone(DateTime utcDateTime)
		{
			TimeZoneInfo timeZone = TimeZoneInfo.Local;
			DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);
			return localTime;
		}
	}
}
