// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AccountServer.Extensions
{
    public class LoginPasswordGrantValidator : IExtensionGrantValidator
    {
        public Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var username = context.Request.Raw.Get("username");
            var password = context.Request.Raw.Get("password");

            if (username != null && password != null)
            {
                context.Result = new GrantValidationResult(
                    subject: "818727",
                    claims: new[] { new Claim("extra_claim", "ttt") },
                    authenticationMethod: GrantType);
            }
            else
            {
                // custom error message
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid login_pwd credential");
            }

            return Task.CompletedTask;
        }

        public string GrantType
        {
            get { return "login_pwd"; }
        }
    }
}