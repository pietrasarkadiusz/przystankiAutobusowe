using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PolaczeniaAutobusowe.Data;
using PolaczeniaAutobusowe.Models;
using PolaczeniaAutobusowe.Utility;

namespace PolaczeniaAutobusowe.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "{0} musi być przynajmniej {2} i maksymalnie {1} znakowe.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Hasło")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Potwierdzone hasło")]
            [Compare("Password", ErrorMessage = "Hasło i potwierdzone hasło nie zgadzają się.")]
            public string ConfirmPassword { get; set; }

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

            public bool IsAdmin { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Imie = Input.Imie,
                    Nazwisko = Input.Nazwisko,
                    Adres = Input.Adres,
                    KodPocztowy = Input.KodPocztowy
                };

                if (!Input.IsAdmin)
                {
                    user.EmailConfirmed = true;
                }

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    if(!await _roleManager.RoleExistsAsync(SD.AdminEndUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.AdminEndUser));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.CustomerEndUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.CustomerEndUser));
                    }
                    if (Input.IsAdmin)
                    {
                        await _userManager.AddToRoleAsync(user, SD.AdminEndUser);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { userId = user.Id, code = code },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Potwierdź twój adres email",
                            $"Potwierdź swoje konto przez <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>kliknij tutaj</a>.");
                        return LocalRedirect("/Users/Index");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, SD.CustomerEndUser);
                        _logger.LogInformation("Użytkownik utworzył nowe konto z hasłem.");
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }

                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
