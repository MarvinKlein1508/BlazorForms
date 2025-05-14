using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Application.Auth;
public static class SaltGenerator
{
    private static byte[] GenerateSalt(int size = 32)
    {
        byte[] salt = new byte[size];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }

    public static string GenerateSaltBase64(int size = 32)
    {
        var salt = GenerateSalt(size);
        return Convert.ToBase64String(salt);
    }
}
