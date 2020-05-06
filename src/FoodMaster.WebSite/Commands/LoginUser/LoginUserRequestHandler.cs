﻿using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using FoodMaster.WebSite.Abstraction.Services;
using MediatR;

namespace FoodMaster.WebSite.Commands.LoginUser
{
    public class LoginUserRequestHandler : IRequestHandler<LoginCredentials, Claim[]>
    {
        private readonly IUsersService usersService;

        public LoginUserRequestHandler(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        public async Task<Claim[]> Handle(LoginCredentials request, CancellationToken cancellationToken)
        {
            var user = usersService.FindByUserName(request.UserName);
            var validPassword = usersService.VerifyPassword(user, request.Password);

            if (user is null || !validPassword)
            {
                return null;
            }

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            return await Task.FromResult(claims);
        }
    }
}