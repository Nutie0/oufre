using System.Security.Cryptography;
using System.Text;

namespace registerAPI.Services;

public class TokenService
{
    private readonly string _secretKey;

    public TokenService(IConfiguration config)
    {
        _secretKey = config["PinSecretKey"];
    }

    public string GenerateVerificationToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(10))
                     .Replace("+", "-")
                     .Replace("/", "_");
    }

    public (string Token, string Pin) GenerateTokenWithPin()
    {
        var token = GenerateVerificationToken();
        var pin = GeneratePinFromToken(token);
        return (token, pin);
    }

    private string GeneratePinFromToken(string token)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
        byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));

        int numericPin = Math.Abs(BitConverter.ToInt32(hash.AsSpan(0, 4))) % 1000000;
        return numericPin.ToString("D4");
    }

    public bool ValidatePin(string token, string pin)
    {
        var expectedPin = GeneratePinFromToken(token);
        return pin == expectedPin;
    }
}