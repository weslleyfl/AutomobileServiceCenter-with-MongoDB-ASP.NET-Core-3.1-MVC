using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ASC.Web.Controllers;
using ASC.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ASC.Utilities;
using ASC.Models.BaseTypes;
using ASC.Web.Models.AccountViewModels;
using ASC.Web.Controllers.Base;
using AspNetCore.Identity.MongoDbCore.Models;

namespace ASC.Web.Controllers
{

    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ServiceEngineers()
        {

            var serviceEngineers = await _userManager.GetUsersInRoleAsync(Roles.Engineer.ToString());

            // Mantenha todos os engenheiros de serviço na sessão
            HttpContext.Session.SetSession("ServiceEngineers", serviceEngineers);

            var engineersViewModel = new ServiceEngineerViewModel
            {
                Registration = new ServiceEngineerRegistrationViewModel() { IsEdit = false },
                ServiceEngineers = serviceEngineers?.ToList()
            };

            return View(engineersViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ServiceEngineers(ServiceEngineerViewModel serviceEngineer)
        {
            serviceEngineer.ServiceEngineers = HttpContext.Session.GetSession<List<ApplicationUser>>("ServiceEngineers");

            if (ModelState.IsValid == false)
            {
                return View(serviceEngineer);
            }

            // If the IsEdit flag is false, a new user is created with the given details and password and is added to the Engineer role.
            bool success = serviceEngineer.Registration.IsEdit
                ? await UpdateEngineer(serviceEngineer)
                : await CreateEngineer(serviceEngineer);

            // Finally, an e-mail is sent to the user with e-mail and password details used to create/modify the account.
            // await SendEmailToUser(serviceEngineer);

            return (success == true)
                ? (IActionResult)RedirectToAction("ServiceEngineers")
                : View(serviceEngineer);

        }

        private async Task<bool> UpdateEngineer(ServiceEngineerViewModel serviceEngineer)
        {
            // Alterar usuario
            ApplicationUser user = await _userManager.FindByEmailAsync(serviceEngineer.Registration.Email);
            user.UserName = serviceEngineer.Registration.UserName;
            IdentityResult result = await _userManager.UpdateAsync(user);

            if (result.Succeeded == false)
            {
                result.Errors.ToList().ForEach((error) => ModelState.AddModelError(string.Empty, error.Description));
                return result.Succeeded;
            }

            // Alterar senha
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            IdentityResult passwordResult = await _userManager.ResetPasswordAsync(user, token, serviceEngineer.Registration.Password);

            if (passwordResult.Succeeded == false)
            {
                passwordResult.Errors.ToList().ForEach((error) => ModelState.AddModelError(string.Empty, error.Description));
                return passwordResult.Succeeded;
            }

            // Alterar claims
            user = await _userManager.FindByEmailAsync(serviceEngineer.Registration.Email);
            MongoClaim isActiveClaim = user.Claims.SingleOrDefault(p => p.Type == "IsActive");
            IdentityResult removeClaimResult = await _userManager.RemoveClaimAsync(user, new Claim(isActiveClaim.Type, isActiveClaim.Value));
            IdentityResult addClaimResult = await _userManager.AddClaimAsync(user, new Claim(isActiveClaim.Type, serviceEngineer.Registration.IsActive.ToString()));

            return true;

        }

        private async Task<bool> CreateEngineer(ServiceEngineerViewModel serviceEngineer)
        {

            // Criar usuario               
            var user = new ApplicationUser
            {
                UserName = serviceEngineer.Registration.UserName,
                Email = serviceEngineer.Registration.Email,
                EmailConfirmed = true
            };

            IdentityResult result = await _userManager.CreateAsync(user, serviceEngineer.Registration.Password);

            if (result.Succeeded)
            {
                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, serviceEngineer.Registration.Email));
                await _userManager.AddClaimAsync(user, new Claim("IsActive", serviceEngineer.Registration.IsActive.ToString()));

                // Assign user to Engineer Role
                IdentityResult roleResult = await _userManager.AddToRoleAsync(user, Roles.Engineer.ToString());

                if (roleResult.Succeeded == false)
                {
                    roleResult.Errors.ToList().ForEach(p => ModelState.AddModelError("", p.Description));
                    return roleResult.Succeeded;
                }

                return result.Succeeded;

            }
            else
            {
                result.Errors.ToList().ForEach(p => ModelState.AddModelError("", p.Description));
                return result.Succeeded;
            }


        }

        private async Task SendEmailToUser(ServiceEngineerViewModel serviceEngineer)
        {
            if (serviceEngineer.Registration.IsActive)
            {
                await _emailSender.SendEmailAsync(serviceEngineer.Registration.Email,
                       "Account Created/Modified",
                       $"Email : {serviceEngineer.Registration.Email} /n Passowrd : {serviceEngineer.Registration.Password}            ");
            }
            else
            {
                await _emailSender.SendEmailAsync(serviceEngineer.Registration.Email,
                     "Account Deactivated",
                     "Your account has been deactivated.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.FindByEmailAsync(HttpContext.User.GetCurrentUserDetails().Email);
            return View(new ProfileViewModel { UserName = user.UserName, IsEditSuccess = false });
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel profile)
        {
            var user = await _userManager.FindByEmailAsync(HttpContext.User.GetCurrentUserDetails().Email);
            user.UserName = profile.UserName;
            var result = await _userManager.UpdateAsync(user);

            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(user, false);

            profile.IsEditSuccess = result.Succeeded;
            AddErrors(result);

            if (ModelState.ErrorCount > 0)
            {
                return View(profile);
            }

            return RedirectToAction(nameof(Profile));

        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Customers()
        {
            IList<ApplicationUser> users = await _userManager.GetUsersInRoleAsync(Roles.User.ToString());
            // Manter os usuarios (clientes) na sessao
            HttpContext.Session.SetSession("Customers", users);

            var customerModel = new CustomersViewModel()
            {
                Customers = users?.ToList(),
                Registration = new CustomerRegistrationViewModel()
            };

            return View(customerModel);

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Customers(CustomersViewModel customer)
        {
            customer.Customers = HttpContext.Session.GetSession<List<ApplicationUser>>("Customers");

            if (ModelState.IsValid == false)
            {
                return View(customer);
            }

            var user = await _userManager.FindByEmailAsync(customer.Registration.Email);

            // Update claims
            var isActiveClaim = user.Claims.SingleOrDefault(p => p.Type == "IsActive");
            var removeClaimResult = await _userManager
                .RemoveClaimAsync(user, new Claim(isActiveClaim.Type, isActiveClaim.Value));
            var addClaimResult = await _userManager
                .AddClaimAsync(user, new Claim(isActiveClaim.Type, customer.Registration.IsActive.ToString()));

            if (customer.Registration.IsActive == false)
            {
                await _emailSender.SendEmailAsync(customer.Registration.Email,
                  "Account Deativated",
                  $"Your account has been Deactivated!!!");
            }

            return RedirectToAction(nameof(Customers));
        }


        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion

    }
}