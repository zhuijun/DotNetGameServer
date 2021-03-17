// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using AccountServer.Data;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AccountServer.Extensions
{
    public class OpenIdGrantValidator : IExtensionGrantValidator
    {
        private readonly AccountServerContext _context;

        public OpenIdGrantValidator(AccountServerContext context)
        {
            _context = context;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var openid = context.Request.Raw.Get("openid");

            if (openid != null)
            {
                var query = from userAccountWx in _context.UserAccountWx
                            where userAccountWx.OpenId == openid
                            join userAccount in _context.UserAccount on userAccountWx.UserId equals userAccount.UserId
                            select userAccount;

                var account = await query.FirstOrDefaultAsync();
                if (account != null)
                {
                    context.Result = new GrantValidationResult(
                        subject: $"{account.UserId}",
                        claims: new[] { new Claim("nickname", $"{account.NickName}"), new Claim("headicon", $"{account.HeadIcon}") },
                        authenticationMethod: GrantType);
                }
                else
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidTarget, "invalid login_openid");
                }
            }
            else
            {
                // custom error message
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid login_openid credential");
            }
        }

        public string GrantType
        {
            get { return "login_openid"; }
        }
    }
}