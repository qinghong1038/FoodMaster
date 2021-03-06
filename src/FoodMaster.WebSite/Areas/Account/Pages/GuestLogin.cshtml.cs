﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using FoodMaster.WebSite.Abstraction.Services;
using FoodMaster.WebSite.Domain;
using FoodMaster.WebSite.Filters;
using FoodMaster.WebSite.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodMaster.WebSite.Areas.Account.Pages
{
    [ServiceFilter(typeof(WriteToDiskFilterAttribute))]
    [BindProperties]
    public class GuestLoginModel : PageModel
    {
        private readonly IUsersService usersService;

        public GuestLoginModel(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        public GuestCredentials GuestCredentials { get; set; }


        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }

            var userId = Guid.NewGuid().ToString();
            var assignedRole = Roles.Guest.ToString();

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, GuestCredentials.FullName),
                new Claim(ClaimTypes.DateOfBirth, GuestCredentials.BirthDate.ToString()),
                new Claim(ClaimTypes.Role, assignedRole)
            };

            var user = new User
            {
                Id = userId,
                UserName = userId,
                FullName = GuestCredentials.FullName,
                BirthDate = GuestCredentials.BirthDate,
                Claims = new List<Claim>(claims),
                Role = assignedRole
            };
            
            usersService.Create(user);

            await HttpContext.SignInAsync(
                new ClaimsPrincipal(
                    new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));

            return RedirectToPagePermanent("Index");
        }


        public async Task<IActionResult> OnGetSignOutAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var user = usersService.Get(u => u.Id == User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (user.Role.Equals(Roles.Guest.ToString()))
            {
                usersService.Delete(user);
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToPage();
        }
    }
}