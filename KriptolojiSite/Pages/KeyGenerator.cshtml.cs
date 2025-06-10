using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KriptolojiSite.Services;

namespace KriptolojiSite.Pages
{
    public class KeyGeneratorModel : PageModel
    {
        private readonly EncryptionService _encryptionService;
        public KeyGeneratorModel(EncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }

        // AES
        [BindProperty] public int AESKeySize { get; set; } = 128;
        [BindProperty] public string? AESKey { get; set; }
        [BindProperty] public string? AESIV { get; set; }

        // RSA
        [BindProperty] public int RSAKeySize { get; set; } = 2048;
        [BindProperty] public string? RSAPublicKey { get; set; }
        [BindProperty] public string? RSAPrivateKey { get; set; }

        public void OnGet()
        {
            AESKey = string.Empty;
            AESIV = string.Empty;
            RSAPublicKey = string.Empty;
            RSAPrivateKey = string.Empty;
        }

        public IActionResult OnPostAES(int AESKeySize)
        {
            this.AESKeySize = AESKeySize;
            var keyIv = _encryptionService.GenerateAESKeyAndIV(AESKeySize);
            if (keyIv.Key == null || keyIv.IV == null)
            {
                ModelState.AddModelError(string.Empty, "Anahtar veya IV üretilemedi!");
                return Page();
            }
            AESKey = keyIv.Key;
            AESIV = keyIv.IV;
            return Page();
        }

        public IActionResult OnPostRSA()
        {
            var keys = _encryptionService.GenerateRSAKeys(RSAKeySize);
            if (keys.PublicKey == null || keys.PrivateKey == null)
            {
                ModelState.AddModelError(string.Empty, "RSA anahtarları üretilemedi!");
                return Page();
            }
            RSAPublicKey = keys.PublicKey;
            RSAPrivateKey = keys.PrivateKey;
            return Page();
        }
    }
} 