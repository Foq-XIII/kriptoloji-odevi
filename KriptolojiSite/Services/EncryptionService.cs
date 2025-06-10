using System.Security.Cryptography;
using System.Text;

namespace KriptolojiSite.Services
{
    public class EncryptionService
    {
        public string EncryptAES(string plainText, string key, string iv, string mode, string padding, int keySize)
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = keySize;
                try
                {
                    aes.Key = Convert.FromBase64String(key);
                    aes.IV = Convert.FromBase64String(iv);
                }
                catch (FormatException)
                {
                    return null;
                }
                // Set encryption mode
                aes.Mode = mode switch
                {
                    "CBC" => CipherMode.CBC,
                    "ECB" => CipherMode.ECB,
                    _ => CipherMode.CBC
                };
                // Set padding mode
                aes.Padding = padding switch
                {
                    "PKCS7" => PaddingMode.PKCS7,
                    "None" => PaddingMode.None,
                    "Zeros" => PaddingMode.Zeros,
                    _ => PaddingMode.PKCS7
                };
                using (var encryptor = aes.CreateEncryptor())
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    if (aes.Padding == PaddingMode.None && (plainBytes.Length % aes.BlockSize / 8 != 0))
                    {
                        // Veri blok boyutunun katı değil, hata!
                        return null;
                    }
                    try
                    {
                        byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                        return Convert.ToBase64String(cipherBytes);
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
        }

        public string DecryptAES(string cipherText, string key, string iv, string mode, string padding, int keySize)
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = keySize;
                aes.Key = Convert.FromBase64String(key);
                aes.IV = Convert.FromBase64String(iv);

                // Set encryption mode
                aes.Mode = mode switch
                {
                    "CBC" => CipherMode.CBC,
                    "ECB" => CipherMode.ECB,
                    _ => CipherMode.CBC
                };

                // Set padding mode
                aes.Padding = padding switch
                {
                    "PKCS7" => PaddingMode.PKCS7,
                    "None" => PaddingMode.None,
                    "Zeros" => PaddingMode.Zeros,
                    _ => PaddingMode.PKCS7
                };

                using (var decryptor = aes.CreateDecryptor())
                {
                    byte[] cipherBytes = Convert.FromBase64String(cipherText);
                    byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                    return Encoding.UTF8.GetString(plainBytes);
                }
            }
        }

        public (string PublicKey, string PrivateKey) GenerateRSAKeys(int keySize)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.KeySize = keySize;
                return (
                    Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo()),
                    Convert.ToBase64String(rsa.ExportRSAPrivateKey())
                );
            }
        }

        public string EncryptRSA(string plainText, string publicKey, string padding)
        {
            using (RSA rsa = RSA.Create())
            {
                try
                {
                    rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKey), out _);
                }
                catch (FormatException)
                {
                    // Hatalı key formatı, null dön
                    return null;
                }
                catch (CryptographicException)
                {
                    // ASN1 veya başka bir key hatası, null dön
                    return null;
                }
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] cipherBytes;
                if (padding == "OAEP")
                {
                    cipherBytes = rsa.Encrypt(plainBytes, RSAEncryptionPadding.OaepSHA256);
                }
                else
                {
                    cipherBytes = rsa.Encrypt(plainBytes, RSAEncryptionPadding.Pkcs1);
                }
                return Convert.ToBase64String(cipherBytes);
            }
        }

        public string DecryptRSA(string cipherText, string privateKey, string padding)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);

                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                byte[] plainBytes;

                if (padding == "OAEP")
                {
                    plainBytes = rsa.Decrypt(cipherBytes, RSAEncryptionPadding.OaepSHA256);
                }
                else
                {
                    plainBytes = rsa.Decrypt(cipherBytes, RSAEncryptionPadding.Pkcs1);
                }

                return Encoding.UTF8.GetString(plainBytes);
            }
        }

        public (string Key, string IV) GenerateAESKeyAndIV(int keySize)
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = keySize;
                aes.GenerateKey();
                aes.GenerateIV();

                return (
                    Convert.ToBase64String(aes.Key),
                    Convert.ToBase64String(aes.IV)
                );
            }
        }
    }
} 