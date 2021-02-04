using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASC.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace ASC.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ConfirmEmailModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(actionName: "Index", controllerName: "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }


            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            // StatusMessage = result.Succeeded ? "Obrigado por confirmar seu e-mail." 
            if (result.Succeeded)
            {
                StatusMessage = "Obrigado por confirmar seu e-mail.";
            }
            else
            {
                string errorDescription = result.Errors?.FirstOrDefault().Description;
                if (errorDescription.Contains("token invalid", StringComparison.InvariantCultureIgnoreCase))
                {
                    await DeleteConfirmedAsync(userId);
                }

                StatusMessage = $"Erro ao confirmar seu e-mail. {errorDescription}";
            }


            return Page();
        }

        private async Task DeleteConfirmedAsync(string userId)
        {

            var user = await _userManager.FindByIdAsync(userId);
            var logins = user.Logins;
            await _userManager.DeleteAsync(user);
            //var result = await _userManager.RemoveLoginAsync(user, logins.FirstOrDefault().LoginProvider, logins.FirstOrDefault().ProviderKey);

        }
    }
}
