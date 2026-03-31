using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProjectHX.Helpers;
public static class FileCryptoHelper
{
	public static void EncryptFile(string inputFile, string outputFile, string encryptionKey)
	{
		byte[] salt = GenerateSalt();
		using (FileStream fsCrypt = new FileStream(outputFile, FileMode.Create))
		{
			byte[] passwordBytes = Encoding.UTF8.GetBytes(encryptionKey);
			using (RijndaelManaged AES = new RijndaelManaged())
			{
				AES.KeySize = 256;
				AES.BlockSize = 128;
				var key = new Rfc2898DeriveBytes(passwordBytes, salt, 1000);
				AES.Key = key.GetBytes(AES.KeySize / 8);
				AES.IV = key.GetBytes(AES.BlockSize / 8);
				AES.Padding = PaddingMode.PKCS7;

				fsCrypt.Write(salt, 0, salt.Length);

				using (CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write))
				{
					using (FileStream fsIn = new FileStream(inputFile, FileMode.Open))
					{
						byte[] buffer = new byte[1048576];
						int read;
						while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
						{
							cs.Write(buffer, 0, read);
						}
					}
					cs.FlushFinalBlock();
				}
			}
		}
	}

	private static byte[] GenerateSalt()
	{
		byte[] data = new byte[32];
		using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
		{
			rng.GetBytes(data);
		}
		return data;
	}

	public static void DecryptFile(string inputFile, string outputFile, string encryptionKey)
	{
		byte[] passwordBytes = Encoding.UTF8.GetBytes(encryptionKey);
		byte[] salt = new byte[32];

		using (FileStream fsCrypt = new FileStream(inputFile, FileMode.Open))
		{
			fsCrypt.Read(salt, 0, salt.Length);
			using (RijndaelManaged AES = new RijndaelManaged())
			{
				AES.KeySize = 256;
				AES.BlockSize = 128;
				var key = new Rfc2898DeriveBytes(passwordBytes, salt, 1000);
				AES.Key = key.GetBytes(AES.KeySize / 8);
				AES.IV = key.GetBytes(AES.BlockSize / 8);
				AES.Padding = PaddingMode.PKCS7;

				using (CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read))
				{
					using (FileStream fsOut = new FileStream(outputFile, FileMode.Create))
					{
						byte[] buffer = new byte[1048576];
						int read;
						while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
						{
							fsOut.Write(buffer, 0, read);
						}
					}
				}
			}
		}
	}
}
