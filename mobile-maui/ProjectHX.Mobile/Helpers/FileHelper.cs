using ProjectHX.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHX.Mobile.Helpers
{
	public partial class FileHelper
	{
		public static partial string GetFileUri(string filePath);
		
		public static async Task<string> CopyFileToInternalStorageAsync(string fileName, string sourcePath)
		{
			//string sourcePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
			string destinationPath = Path.Combine(FileSystem.AppDataDirectory, "copiedFiles", fileName);

			Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!); // Ensure directory exists

			using (var sourceStream = File.Open(sourcePath, FileMode.Open))
			using (var destinationStream = File.Create(destinationPath))
			{
				await sourceStream.CopyToAsync(destinationStream);
			}

			return destinationPath;
		}
		
		public static string DecryptToInternalStorageAsync(string fileName, string sourcePath, string encryptionKey)
		{
			//string sourcePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
			string destinationPath = Path.Combine(FileSystem.AppDataDirectory, "copiedFiles", fileName);

			Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!); // Ensure directory exists

			/*using (var sourceStream = File.Open(sourcePath, FileMode.Open))
			using (var destinationStream = File.Create(destinationPath))
			{
				await sourceStream.CopyToAsync(destinationStream);
			}*/

			//string newDestinationPath = Path.Combine(Path.get(destinationPath), Path.GetFileName(destinationPath));

			FileCryptoHelper.DecryptFile(sourcePath, destinationPath, encryptionKey);

			//if (File.Exists(destinationPath))
			//{
			//	File.Delete(destinationPath);
			//}

			return destinationPath;
		}

		public static string GetFileUrl(string filePath)
		{
			return "file://" + filePath.Replace("\\", "/");
		}
	}
}

