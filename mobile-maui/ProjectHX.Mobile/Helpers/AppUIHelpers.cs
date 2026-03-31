using ProjectHX.Mobile.Contexts;
using System.Text.RegularExpressions;
using System.Windows.Input;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Alerts;
using Microsoft.EntityFrameworkCore;

namespace ProjectHX.Mobile.Helpers
{
	public partial class AppUIHelpers
	{
		public static string RoleTypeKey { get; set; } = "RoleType";
		public static string AppPrimaryColor { get; set; } = "#FD5E0F";
		public static string AppSecondaryColor { get; set; } = "#310979";
		public static string AppColorWhite { get; set; } = "#FFFFFF";
		public static string AppTertiaryColor1 { get; set; } = "#F9C5AD";
		public static string BackgroundGreen { get; set; } = "#2b823b";
		public static string DarkGreen { get; set; } = "#25472c";
		public static string LogoGreen { get; set; } = "#93c242";

		// Define a delegate that matches the signature of the callback method
		//public delegate void MyCallback(string message, int number);
		public delegate void NetworkCallback();

		// Method that takes a callback as a parameter
		public void CallNetworkCallback(NetworkCallback callback)
		{
			// Call the callback method
			callback();
		}

		public enum LoadingType
		{
			Loading,
			Reloading,
			Updating
		}

		public static async void ShortToast(string message)
		{
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

			string text = message;
			ToastDuration duration = ToastDuration.Short;
			double fontSize = 14;

			var toast = Toast.Make(text, duration, fontSize);

			await toast.Show(cancellationTokenSource.Token);
		}

		public static async void LongToast(string message)
		{
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

			string text = message;
			ToastDuration duration = ToastDuration.Long;
			double fontSize = 14;

			var toast = Toast.Make(text, duration, fontSize);

			await toast.Show(cancellationTokenSource.Token);
		}

		public static async Task ImportSqlDump(SqliteDbContext dbContext, string sqlFilePath)
		{
			if (Preferences.Get("SqlImported", false))
				return;

			using var stream = await FileSystem.OpenAppPackageFileAsync(sqlFilePath);
			using var reader = new StreamReader(stream);
			string content = await reader.ReadToEndAsync();

			var sqlCommands = content; // File.ReadAllText(sqlFilePath);

			// Split commands by semicolon
			var commands = sqlCommands.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

			foreach (var command in commands)
			{
				// Skip empty commands
				if (string.IsNullOrWhiteSpace(command)) continue;
				try
				{
					// Execute the command
					dbContext.Database.ExecuteSqlRaw(command.Last() == ';' ? command : $"{command};");
				}
				catch (Exception)
				{
					return;
				}
			}

			Preferences.Set("SqlImported", true);
		}
	}
}
