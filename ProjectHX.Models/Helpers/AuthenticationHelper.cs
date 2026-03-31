using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;

namespace ProjectHX.Helpers
{
	public static class AuthenticationHelper
	{
		public static string AuthenticationKey { get; private set; } = "Skylon Authencation for account security and data protection with bla bla and bla bla bla";
		private static int _keySize { get; } = 128;
		private static int _iterations { get; } = 350000;
		public static string GenerateGuidString()
		{
			return GenerateGuid().ToString();
		}

		public static Guid GenerateGuid()
		{
			return Guid.NewGuid();
		}

		public static bool VerifyPassword(string password, string hash, byte[] salt)
		{
			var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, _iterations, HashAlgorithmName.SHA512, _keySize);

			return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
		}

		public static int GenerateOTP()
		{
			Random random = new();
			int randomNumber = random.Next(1000000);
			string sixDigits = randomNumber.ToString("D6");
			return int.Parse(sixDigits);
		}

		public static string GenerateHash(string source)
		{
			byte[] tmpSource = Encoding.ASCII.GetBytes(source); // Convert to byte array
			byte[] tmpHash = MD5.HashData(tmpSource); // Compute MD5 hash

			return ByteArrayToString(tmpHash);
		}
		public static string ByteArrayToString(byte[] arrInput)
		{
			StringBuilder sOutput = new StringBuilder(arrInput.Length);
			for (int i = 0; i < arrInput.Length; i++)
			{
				sOutput.Append(arrInput[i].ToString("X2")); // Convert each byte to hexadecimal
			}
			return sOutput.ToString();
		}

		public static bool CompareMD5Hash(string hash1, string hash2)
		{
			return string.Equals(hash1, hash2, StringComparison.OrdinalIgnoreCase);
		}

		public static long GetCurrentUnixTimestampMillis()
		{
			DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return (long)(DateTime.UtcNow - epochStart).TotalMilliseconds;
		}
	}
}
