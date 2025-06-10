using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KriptolojiSite.Services;

namespace KriptolojiSite.Pages
{
    public class HashModel : PageModel
    {
        private readonly HashService _hashService;
        public HashModel(HashService hashService)
        {
            _hashService = hashService;
        }

        [BindProperty]
        public string InputText { get; set; }
        [BindProperty]
        public IFormFile InputFile { get; set; }
        [BindProperty]
        public string Algorithm { get; set; } = "SHA256";
        [BindProperty]
        public string OutputFormat { get; set; } = "hex";

        public string HashResult { get; set; }
        public int HashLength { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (InputFile != null && InputFile.Length > 0)
            {
                HashResult = await _hashService.ComputeFileHashAsync(InputFile, Algorithm, OutputFormat);
                if (HashResult == null)
                {
                    ErrorMessage = "Dosya hash'lenirken bir hata oluştu. Lütfen geçerli ve küçük boyutlu bir dosya yükleyin.";
                }
            }
            else if (!string.IsNullOrWhiteSpace(InputText))
            {
                HashResult = _hashService.ComputeHash(InputText, Algorithm, OutputFormat);
            }
            else
            {
                HashResult = string.Empty;
            }

            HashLength = HashResult?.Length ?? 0;
            return Page();
        }
    }
} 