using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PolaczeniaAutobusowe.Data;

namespace PolaczeniaAutobusowe.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _db;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender,
            ApplicationDbContext db)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Phone]
            [Display(Name = "Numer telefonu")]
            public string PhoneNumber { get; set; }
            [Required]
            [RegularExpression(@"^[a-zA-Z]+$",
            ErrorMessage = "Znaki i cyfry są niedozwolone.")]
            public string Imie { get; set; }
            [Required]
            [RegularExpression(@"^[a-zA-Z]+$",
            ErrorMessage = "Znaki i cyfry są niedozwolone.")]
            public string Nazwisko { get; set; }
            public string Adres { get; set; }
            public string KodPocztowy { get; set; }
            
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Nie można załadować użytkownika z ID '{_userManager.GetUserId(User)}'.");
            }

            var userFromDb = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Email == user.Email);

            Username = userFromDb.UserName;

            Input = new InputModel
            {
                Imie = userFromDb.Imie,
                Nazwisko = userFromDb.Nazwisko,
                Adres = userFromDb.Adres,
                KodPocztowy = userFromDb.KodPocztowy,
                PhoneNumber = userFromDb.PhoneNumber,
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Nie można załadować użytkownika z ID '{_userManager.GetUserId(User)}'.");
            }

            var userFromDb = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Email == user.Email);

            userFromDb.Imie = Input.Imie;
            userFromDb.Nazwisko = Input.Nazwisko;
            userFromDb.Adres = Input.Adres;
            userFromDb.KodPocztowy = Input.KodPocztowy;
            userFromDb.PhoneNumber = Input.PhoneNumber;
            await _db.SaveChangesAsync();

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Twój profil został zaktualizowany";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Nie można załadować użytkownika z ID '{_userManager.GetUserId(User)}'.");
            }


            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                email,
                "Potwierdź adres email",
                $"Potwierdź swoje konto przez <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>kliknij tutaj</a>.");

            StatusMessage = "Weryfikacja adresu email została wysłana. Sprawdź swój adres email.";
            return RedirectToPage();
        }
    }
}
