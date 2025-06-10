using System.Security.Cryptography;
using System.Text;

namespace KriptolojiSite.Services
{
    public class HashService
    {
        public string ComputeHash(string input, string algorithm, string outputFormat)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes;

            using (HashAlgorithm hashAlgorithm = algorithm switch
            {
                "SHA1" => SHA1.Create(),
                "SHA256" => SHA256.Create(),
                "SHA384" => SHA384.Create(),
                "SHA512" => SHA512.Create(),
                _ => SHA256.Create()
            })
            {
                hashBytes = hashAlgorithm.ComputeHash(inputBytes);
            }

            return outputFormat == "base64" 
                ? Convert.ToBase64String(hashBytes)
                : BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        public async Task<string> ComputeFileHashAsync(IFormFile file, string algorithm, string outputFormat)
        {
            if (file == null || file.Length == 0)
                return null;
            try
            {
                using (var stream = file.OpenReadStream())
                {
                    byte[] hashBytes;
                    using (HashAlgorithm hashAlgorithm = algorithm switch
                    {
                        "SHA1" => SHA1.Create(),
                        "SHA256" => SHA256.Create(),
                        "SHA384" => SHA384.Create(),
                        "SHA512" => SHA512.Create(),
                        _ => SHA256.Create()
                    })
                    {
                        hashBytes = await hashAlgorithm.ComputeHashAsync(stream);
                    }
                    return outputFormat == "base64"
                        ? Convert.ToBase64String(hashBytes)
                        : BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                }
            }
            catch (Exception ex)
            {
                // Hata loglanabilir: ex.ToString()
                return null;
            }
        }

        public int GetHashLength(string algorithm)
        {
            return algorithm switch
            {
                "SHA1" => 40,    // 160 bits = 40 hex characters
                "SHA256" => 64,  // 256 bits = 64 hex characters
                "SHA384" => 96,  // 384 bits = 96 hex characters
                "SHA512" => 128, // 512 bits = 128 hex characters
                _ => 64
            };
        }
    }
} 