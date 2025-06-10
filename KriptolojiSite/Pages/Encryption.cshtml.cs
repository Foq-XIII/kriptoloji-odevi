using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KriptolojiSite.Services;
using System.Text;

namespace KriptolojiSite.Pages
{
    public class EncryptionModel : PageModel
    {
        private readonly EncryptionService _encryptionService;
        public EncryptionModel(EncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }

        // AES
        [BindProperty] public string AESMode { get; set; } = "CBC";
        [BindProperty] public string AESPadding { get; set; } = "PKCS7";
        [BindProperty] public int AESKeySize { get; set; } = 128;
        [BindProperty] public string AESText { get; set; }
        [BindProperty] public string AESKey { get; set; }
        [BindProperty] public string AESIV { get; set; }
        [BindProperty] public string AESEncrypted { get; set; }
        [BindProperty] public string AESDecrypted { get; set; }

        // RSA
        [BindProperty] public int RSAKeySize { get; set; } = 2048;
        [BindProperty] public string RSAPadding { get; set; } = "OAEP";
        [BindProperty] public string RSAPublicKey { get; set; }
        [BindProperty] public string RSAText { get; set; }
        [BindProperty] public string RSAPrivateKey { get; set; }
        [BindProperty] public string RSAEncrypted { get; set; }
        [BindProperty] public string RSADecrypted { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPostAES(string AESGenerateKey, string AESGenerateIV)
        {
            if (!string.IsNullOrEmpty(AESGenerateKey))
            {
                var keyIv = _encryptionService.GenerateAESKeyAndIV(AESKeySize);
                AESKey = keyIv.Key;
                AESIV = keyIv.IV;
                return Page();
            }
            if (!string.IsNullOrEmpty(AESGenerateIV))
            {
                var keyIv = _encryptionService.GenerateAESKeyAndIV(AESKeySize);
                AESIV = keyIv.IV;
                return Page();
            }
            if (!string.IsNullOrWhiteSpace(AESText) && !string.IsNullOrWhiteSpace(AESKey) && !string.IsNullOrWhiteSpace(AESIV))
            {
                AESEncrypted = _encryptionService.EncryptAES(AESText, AESKey, AESIV, AESMode, AESPadding, AESKeySize);
                if (AESEncrypted == null)
                {
                    if (AESPadding == "None" && (AESText == null || Encoding.UTF8.GetBytes(AESText).Length % 16 != 0))
                        ViewData["AES_Error"] = "Padding 'None' seçiliyse, metin uzunluğu 16'nın tam katı olmalıdır!";
                    else
                        ViewData["AES_Error"] = "Şifreleme sırasında bir hata oluştu. Lütfen girişlerinizi kontrol edin.";
                }
            }
            return Page();
        }

        public IActionResult OnPostRSA()
        {
            if (!string.IsNullOrWhiteSpace(RSAText) && !string.IsNullOrWhiteSpace(RSAPublicKey))
            {
                RSAEncrypted = _encryptionService.EncryptRSA(RSAText, RSAPublicKey, RSAPadding);
                if (RSAEncrypted == null)
                {
                    ViewData["RSA_Error"] = "Geçersiz public key formatı! Lütfen doğru bir Base64 public key girin.";
                }
            }
            return Page();
        }

        public IActionResult OnPostAESDecrypt()
        {
            ViewData["ActiveAesTab"] = "decrypt";
            if (!string.IsNullOrWhiteSpace(AESEncrypted) && !string.IsNullOrWhiteSpace(AESKey) && !string.IsNullOrWhiteSpace(AESIV))
            {
                try
                {
                    AESDecrypted = _encryptionService.DecryptAES(AESEncrypted, AESKey, AESIV, AESMode, AESPadding, AESKeySize);
                }
                catch
                {
                    ViewData["AES_Error"] = "Çözme sırasında bir hata oluştu. Lütfen girişlerinizi kontrol edin.";
                }
            }
            return Page();
        }

        public IActionResult OnPostRSADecrypt()
        {
            ViewData["ActiveRsaTab"] = "decrypt";
            if (!string.IsNullOrWhiteSpace(RSAEncrypted) && !string.IsNullOrWhiteSpace(RSAPrivateKey))
            {
                try
                {
                    RSADecrypted = _encryptionService.DecryptRSA(RSAEncrypted, RSAPrivateKey, RSAPadding);
                }
                catch
                {
                    ViewData["RSA_Error"] = "Çözme sırasında bir hata oluştu. Lütfen girişlerinizi kontrol edin.";
                }
            }
            return Page();
        }
    }
} 