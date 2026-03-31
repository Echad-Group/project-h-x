namespace System
{
	public static class Extentions
	{
		public static bool IsNullOrEmpty(this string s) => string.IsNullOrEmpty(s);

		public static bool IsNullOrWhiteSpaces(this string s) => string.IsNullOrWhiteSpace(s);

		public static bool IsNotValid(this string s) => IsNullOrEmpty(s) || IsNullOrWhiteSpaces(s);


		public static string ConvertToPascalCase(this string s)
		{
			if (string.IsNullOrWhiteSpace(s))
				return string.Empty;

			// Split the string by underscores or spaces
			var words = s.Split(new[] { '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);

			// Capitalize the first letter of each word and concatenate them
			for (int i = 0; i < words.Length; i++)
			{
				if (words[i].Length > 0)
				{
					// Capitalize the first letter and make the rest lower case
					words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
				}
			}

			// Join the words to form the Pascal case string
			return string.Join(string.Empty, words);
		}
	}
}