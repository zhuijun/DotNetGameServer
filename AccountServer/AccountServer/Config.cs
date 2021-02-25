// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("api1", "My API"),
                new ApiScope("game", new List<string>{"nickname", "headicon"}),
            };

        public static IEnumerable<Client> Clients =>
            new Client[] 
            {
                // machine to machine client (from quickstart 1)
                new Client
                {
                    ClientId = "client",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = { "api1" }
                },
                new Client
                {
                    ClientId = "client_pwd",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    RequireClientSecret = false,
                    //// secret for authentication
                    //ClientSecrets =
                    //{
                    //    new Secret("secret".Sha256())
                    //},

                    // scopes that client has access to
                    AllowedScopes = { "api1" }
                },
                new Client
                {
                    ClientId = "login_pwd",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = { "login_pwd" },
                    RequireClientSecret = false,
                    //// secret for authentication
                    //ClientSecrets =
                    //{
                    //    new Secret("secret".Sha256())
                    //},

                    // scopes that client has access to
                    AllowedScopes = { "game", "api1" }
                },
                 new Client
                {
                    ClientId = "login_token",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = { "login_token" },
                    RequireClientSecret = false,
                    //// secret for authentication
                    //ClientSecrets =
                    //{
                    //    new Secret("secret".Sha256())
                    //},

                    // scopes that client has access to
                    AllowedScopes = { "game", "api1" }
                },
            };
    }
}