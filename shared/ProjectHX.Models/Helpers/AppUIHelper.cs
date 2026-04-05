using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.Text.RegularExpressions;

namespace ProjectHX.Helpers
{
	public partial class AppUIHelper
	{
		public static string AppTertiaryColor { get; set; } = "#60C185";
		public static string AppUtinaryColor { get; set; } = "#8ED67D";
		public static string AppDenaryColor { get; set; } = "#C1E875";
		public static string AppAccentColor { get; set; } = "#F9F871";
		public static string AppTertiaryColor1 { get; set; } = "#F9C5AD";

		public static string AppPrimaryColor { get; set; } = "#FD5E0F";
		public static string AppSecondaryColor { get; set; } = "#310979";
		public static string AppColorWhite { get; set; } = "#FFFFFF";

		public static Regex PhoneNumberRegex { get; set; } = PhoneRegex();
		public static Regex PasswordRegex { get; set; } = PassRegex();
		public static string EmailRegex { get; } = @"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$";
		public static string NumberRegex { get; } = @"^\d{9}$";
		public static string InputPasswordRegex { get; } = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d][^\\W_]{5,}$";

		[GeneratedRegex(@"^\d{9}$")]
		private static partial Regex PassRegex();
		[GeneratedRegex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{8,}$")]
		private static partial Regex PhoneRegex();
		
		public static string ImageToThumbnailBase64(string imagePath)
		{
			try
			{
				string base64String = string.Empty;
				using (Image image = Image.Load(imagePath))
				{
					image.Mutate(x => x.Resize(image.Width / 8, image.Height / 8));
					base64String = image.ToBase64String(JpegFormat.Instance); //(fullProfileImageThumbnailPath);
				}
				return base64String;
			}
			catch (Exception)
			{
				return $"assets/images/user-def.png";
			}
		}
		public static string StreamToBase64(Stream stream)
		{
			try
			{
				string base64String = string.Empty;
				using (Image image = Image.Load(stream))
				{
					base64String = image.ToBase64String(JpegFormat.Instance); //(fullProfileImageThumbnailPath);
				}
				return base64String;
			}
			catch
			{
				return $"assets/images/user-def.png";
			}
		}

		public static string StreamToThumbnailBase64Async(Stream stream)
		{
			try
			{
				string base64String = string.Empty;
				using (Image image = Image.Load(stream))
				{
					image.Mutate(x => x.Resize(image.Width / 8, image.Height / 8));
					base64String = image.ToBase64String(JpegFormat.Instance); //(fullProfileImageThumbnailPath);
				}
				return base64String;
			}
			catch
			{
				return $"assets/images/user-def.png";
			}
		}

		public static string GenerateColor()
		{
			Random random = new();
			string color = string.Format("#{0:X6}", random.Next(0x1000000)); // generates a random number between 0x0 and 0xFFFFFF and converts it to hexadecimal
			return color;
		}

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

	}
}
