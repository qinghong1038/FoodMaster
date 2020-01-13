﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FoodMaster.WebSite.Abstraction.Services;
using FoodMaster.WebSite.Domain;
using FoodMaster.WebSite.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodMaster.WebSite
{
    public class RegisterModel : PageModel
    {
        private readonly IUsersService usersService;

        public RegisterModel(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        [BindProperty]
        public UserDetails UserDetails { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }

            var userId = Guid.NewGuid().ToString();

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
            };

            var user = new User
            {
                Id = userId,
                FullName = UserDetails.FullName,
                BirthDate = UserDetails.BirthDate,
                Claims = new List<Claim>(claims)
            };

            usersService.Create(user, UserDetails.Password);

            await HttpContext.SignInAsync(
                new ClaimsPrincipal(
                    new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));

            return RedirectPermanent("/Index");
        }

    }
}