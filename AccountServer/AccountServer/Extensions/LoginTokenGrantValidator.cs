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
    public class LoginTokenGrantValidator : IExtensionGrantValidator
    {
        private readonly AccountServerContext _context;

        public LoginTokenGrantValidator(AccountServerContext context)
        {
            _context = context;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var userid = context.Request.Raw.Get("userid");
            var token = context.Request.Raw.Get("token");

            if (userid != null && token != null)
            {
                var account = await _context.UserAccount.AsNoTracking().FirstOrDefaultAsync(a => a.UserID.ToString() == userid && a.Token == token);
                if (account != null)
                {
                    context.Result = new GrantValidationResult(
                        subject: $"{account.UserID}",
                        claims: new[] { new Claim("nickname", $"{account.NickName}") },
                        authenticationMethod: GrantType);
                }
                else
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidTarget, "invalid userid or token");
                }
            }
            else
            {
                // custom error message
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid login_token credential");
            }
        }

        public string GrantType
        {
            get { return "login_token"; }
        }
    }
}