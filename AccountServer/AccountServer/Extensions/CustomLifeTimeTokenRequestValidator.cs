using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Validation;

namespace AccountServer.Extensions
{
    public class CustomLifeTimeTokenRequestValidator : ICustomTokenRequestValidator
    {
        public Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            context.Result.ValidatedRequest.AccessTokenLifetime = TimeSpan.FromSeconds(10).Seconds;

            return Task.CompletedTask;
        }
    }
}