using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ASC.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using ASC.Models.BaseTypes;

namespace ASC.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<ExternalLoginModel> logger,
            IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ProviderDisplayName { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public string LoginProvider { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        /// <summary>
        /// On successful login at the external authentication provider, the browser will be redirected to the callback URL
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="remoteError"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (remoteError != null)
            {
                ErrorMessage = $"Erro de provedor externo: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            // pegar informaçoes do provedor google por exemplo
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Erro ao carregar informações de login externo.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);

                var user = await _userManager.FindByEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email));
                if (user == null)
                {             
                    ErrorMessage = "Tentativa de login inválida.";
                    return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
                }

                var isActive = Convert.ToBoolean(user.Claims.SingleOrDefault(p => p.Type.Equals("IsActive")).Value);
                if (isActive == false)
                {
                    ErrorMessage = "A conta foi bloqueada.";
                    return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
                }

                //return LocalRedirect(returnUrl);

                if ((!string.IsNullOrWhiteSpace(returnUrl)) && (!returnUrl.Equals("/")))
                    return LocalRedirect(returnUrl);
                else
                    return RedirectToAction("Dashboard", "Dashboard");
            }

            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl;
                ProviderDisplayName = info.ProviderDisplayName;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    Input = new InputModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    };
                }
                return Page();
            }
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Erro ao carregar informações de login externo durante a confirmação.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid == false)
            {
                ErrorMessage = "Erro ao carregar os dados da tela.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            // Criar login externo 
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = info.Principal.Identity.Name,
                    Email = email
                };

                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {

                    result = await _userManager.AddLoginAsync(user, info);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                       
                        var callbackUrl = EmailConfirmationLink(userId, code, Request.Scheme);

                        await SendEmailConfirmationAsync(Input.Email, callbackUrl);

                        // Informaçoes complementareas para o Claim do usuario
                        await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, user.Email));
                        await _userManager.AddClaimAsync(user, new Claim("IsActive", "False"));

                        // Add to USER roles
                        await _userManager.AddToRoleAsync(user, Roles.User.ToString());

                        // If account confirmation is required, we need to show the link if we don't have a real email sender
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("./RegisterConfirmation", new { Email = Input.Email });
                        }
                                               

                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);

                        return LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }

            // If account confirmation is required, we need to show the link if we don't have a real email sender
            if (_userManager.Options.SignIn.RequireConfirmedAccount)
            {
                return RedirectToPage("./RegisterConfirmation", new { Email = Input.Email });
            }

            ProviderDisplayName = info.ProviderDisplayName;
            ReturnUrl = returnUrl;
            return Page();


        }

        private string EmailConfirmationLink(string userId, string code, string scheme)
        {
           
            var result = Url.Page(
                           "/Account/ConfirmEmail",
                           pageHandler: null,
                           values: new { area = "Identity", userId = userId, code = code },
                           protocol: scheme);

            return result;
        }

        private async Task SendEmailConfirmationAsync(string email, string callbackUrl)
        {
            await _emailSender.SendEmailAsync(email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        }

    }
}