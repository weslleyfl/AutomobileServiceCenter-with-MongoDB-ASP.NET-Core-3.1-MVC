﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ASC.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();

            _logger.LogInformation("User logged out.");

            
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction(actionName: "Index", controllerName: "Home");
            }

            //return (!string.IsNullOrWhiteSpace(returnUrl))
            //    ? LocalRedirect(returnUrl)
            //    : (IActionResult)RedirectToAction(actionName: "Index", controllerName: "Home");
        }
    }
}
