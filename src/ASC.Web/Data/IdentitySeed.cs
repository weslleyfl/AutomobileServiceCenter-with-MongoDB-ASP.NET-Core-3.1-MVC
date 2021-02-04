using ASC.Web.Configuration;
using ASC.Web.Data.Interfaces;
using ASC.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ASC.Models.BaseTypes;

namespace ASC.Web.Data
{
    public class IdentitySeed : IIdentitySeed
    {
        public async Task Seed(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<ApplicationSettings> options)
        {
            // Obter todos os grupos
            var roles = options.Value.Roles.Split(',');

            // Criar grupos se não existir
            foreach (var role in roles)
            {
                if (await roleManager.RoleExistsAsync(role) == false)
                {
                    var storageRole = new ApplicationRole(role);

                    // crie as funções (Roles) e envie-as para o banco de dados
                    IdentityResult roleResult = await roleManager.CreateAsync(storageRole);
                }
            }

            // Crie um administrador se ele não existir
            ApplicationUser admin = await userManager.FindByEmailAsync(options.Value.AdminEmail);
            if (admin == null)
            {
                var user = new ApplicationUser()
                {
                    UserName = options.Value.AdminName,
                    Email = options.Value.AdminEmail,
                    EmailConfirmed = true
                };

                IdentityResult result = await userManager.CreateAsync(user, options.Value.AdminPassword);
                await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, options.Value.AdminEmail));
                await userManager.AddClaimAsync(user, new Claim("IsActive", "True"));

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, Roles.Admin.ToString());
                }

            }

            // Crie um engenheiro (service engineer) se ele não existir
            ApplicationUser engineer = await userManager.FindByEmailAsync(options.Value.EngineerEmail);
            if (engineer == null)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = options.Value.EngineerName,
                    Email = options.Value.EngineerEmail,
                    EmailConfirmed = true,
                    LockoutEnabled = false
                };

                IdentityResult result = await userManager.CreateAsync(user, options.Value.EngineerPassword);
                await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, options.Value.EngineerEmail));
                await userManager.AddClaimAsync(user, new Claim("IsActive", "True"));

                // Adicionando o engenheiro para o grupo Engineer (Engineer role)
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, Roles.Engineer.ToString());
                }

            }

            // Criar um cliente se ele nao exister
            ApplicationUser customer = await userManager.FindByEmailAsync("teste@teste.com.br");
            if (customer == null)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = "João Cristo" ,
                    Email = "teste@teste.com.br",
                    EmailConfirmed = true,
                    LockoutEnabled = false
                };

                IdentityResult result = await userManager.CreateAsync(user, "123456");
                await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, "teste@teste.com.br"));
                await userManager.AddClaimAsync(user, new Claim("IsActive", "True"));

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, Roles.User.ToString());
                }
            }

        }

    }
}
