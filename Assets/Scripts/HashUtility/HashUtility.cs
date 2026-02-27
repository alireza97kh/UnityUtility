using System.Security.Cryptography;
using System.Text;

public static class HashUtility
{
	public static string Hash(string input)
	{
		using var sha = SHA1.Create();

		byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));

		StringBuilder builder = new();

		foreach (var b in hash)
			builder.Append(b.ToString("x2"));

		return builder.ToString();
	}
}