// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using AccountServer.Data;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AccountServer.Extensions
{
    public class LoginPasswordGrantValidator : IExtensionGrantValidator
    {
        private readonly AccountServerContext _context;

        public LoginPasswordGrantValidator(AccountServerContext context)
        {
            _context = context;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var username = context.Request.Raw.Get("username");
            var password = context.Request.Raw.Get("password");

            if (username != null && password != null)
            {
                var account = await _context.UserAccount.AsNoTracking().FirstOrDefaultAsync(a => a.UserName == username && a.Password == password);
                if (account != null)
                {
                    context.Result = new GrantValidationResult(
                        subject: $"{account.UserID}",
                        claims: new[] { new Claim("nickname", $"{account.NickName}") },
                        authenticationMethod: GrantType);
                }
                else
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidTarget, "invalid username or password");
                }
            }
            else
            {
                // custom error message
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid login_pwd credential");
            }
        }

        public string GrantType
        {
            get { return "login_pwd"; }
        }
    }
}