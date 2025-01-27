using System.Security.Cryptography;
using System.Text;

namespace Gacfox.Wealthome.Utils;

public static class HashUtil
{
    public static string GetPasswordHash(string password, string salt)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
        StringBuilder hex = new StringBuilder();
        foreach (byte b in hashBytes)
        {
            hex.Append(b.ToString("x2"));
        }

        return hex.ToString();
    }
}