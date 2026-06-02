using System.Text;

namespace ECommerceOS.AuthService.Infrastructure.Security;

public class EncryptionService(IOptionsMonitor<KeyOptions> options) : IEncryptionService
{
    private readonly byte[] _key = SHA256.HashData(Encoding.UTF8.GetBytes(options.CurrentValue.Key));
    
    public string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        
        aes.GenerateIV();
        
        using var msEncrypt = new MemoryStream();
        
        msEncrypt.Write(aes.IV, 0, aes.IV.Length);

        using var encryptor = aes.CreateEncryptor();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        csEncrypt.Write(plainBytes, 0, plainBytes.Length);
        csEncrypt.FlushFinalBlockAsync();
        
        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    public string Decrypt(string cipherText)
    {
        byte[] combinedBytes = Convert.FromBase64String(cipherText);

        if (combinedBytes.Length <= 16)
        {
            throw new ArgumentException("invalid cipher length");
        }

        using var aes = Aes.Create();
        aes.Key = _key;
        
        byte[] iv = new byte[16];
        Array.Copy(combinedBytes, 0, iv, 0, 16);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        
        using var msDecrypt = new MemoryStream(combinedBytes, 16, combinedBytes.Length - 16);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);
        
        return srDecrypt.ReadToEnd();
    }
}