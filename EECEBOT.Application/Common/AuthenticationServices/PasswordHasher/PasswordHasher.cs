using System.Security.Cryptography;

namespace EECEBOT.Application.Common.AuthenticationServices.PasswordHasher;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 128 / 8;
    private const int KeySize = 256 / 8;
    private const int Iterations = 10000;
    private const char Separator = ';';
    
    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize);
        return string.Join(Separator,
            Convert.ToBase64String(salt),
            Convert.ToBase64String(key));
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        var parts = hashedPassword.Split(Separator);

        var salt = Convert.FromBase64String(parts[0]);

        var key = Convert.FromBase64String(parts[1]);

        var testKey = Rfc2898DeriveBytes.Pbkdf2(password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize);

        return CryptographicOperations.FixedTimeEquals(key, testKey);
    }
}