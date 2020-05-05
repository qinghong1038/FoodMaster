﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using FoodMaster.WebSite.Abstraction.Services;
using FoodMaster.WebSite.Domain;
using FoodMaster.WebSite.Models;

using MediatR;

namespace FoodMaster.WebSite.Commands
{

    public class CreateGuestCommandHandler : IRequestHandler<GuestCredentials, Claim[]>
    {
        private readonly IUsersService usersService;

        public CreateGuestCommandHandler(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        public async Task<Claim[]> Handle(GuestCredentials request, CancellationToken cancellationToken)
        {
            var userId = Guid.NewGuid().ToString();
            var assignedRole = Roles.Guest.ToString();

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, request.FullName),
                new Claim(ClaimTypes.DateOfBirth, request.BirthDate.ToString()),
                new Claim(ClaimTypes.Role, assignedRole)
            };

            var user = new User
            {
                Id = userId,
                UserName = userId,
                FullName = request.FullName,
                BirthDate = request.BirthDate,
                Claims = new List<Claim>(claims),
                Role = assignedRole
            };

            usersService.Create(user);

            return await Task.FromResult(claims);
        }
    }
}
