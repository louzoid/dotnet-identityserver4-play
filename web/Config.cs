using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace web
{
    public class Config {

        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "alice",
                    Password = "password",

                    Claims = new []
                    {
                        new Claim("name", "Alice"),
                        new Claim("website", "https://alice.com")
                    }
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "bob",
                    Password = "password"
                }
            };
        }
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API")
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                // new Client
                // {
                //     ClientId = "client",

                //     // no interactive user, use the clientid/secret for authentication
                //     AllowedGrantTypes = GrantTypes.ClientCredentials,

                //     // secret for authentication
                //     ClientSecrets =
                //     {
                //         new Secret("secret".Sha256())
                //     },

                //     // scopes that client has access to
                //     AllowedScopes = { "api1" }
                // },
                // resource owner password grant client
                // new Client
                // {
                //     ClientId = "ro.client",
                //     AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                //     ClientSecrets =
                //     {
                //         new Secret("secret".Sha256())
                //     },
                //     AllowedScopes = { "api1" }
                // },
                // OpenID Connect implicit flow client (MVC)
                // new Client
                // {
                //     ClientId = "mvc",
                //     ClientName = "MVC Client",
                //     AllowedGrantTypes = GrantTypes.Implicit,

                //     // where to redirect to after login
                //     RedirectUris = { "http://localhost:5003/signin-oidc" },

                //     // where to redirect to after logout
                //     PostLogoutRedirectUris = { "http://localhost:5003/signout-callback-oidc" },

                //     AllowedScopes = new List<string>
                //     {
                //         IdentityServerConstants.StandardScopes.OpenId,
                //         IdentityServerConstants.StandardScopes.Profile
                //     }
                // },
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    RedirectUris           = { "http://localhost:5003/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5003/signout-callback-oidc" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    },
                    AllowOfflineAccess = true
                },
                new Client
                {
                    ClientId = "deviceClient",
                    ClientName = "Device flow client",
                    AllowedGrantTypes = GrantTypes.DeviceFlow,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    },
                    //means app can use refresh tokens. if true, user can revoke in grants UI
                    AllowOfflineAccess = true
                },
            };
        }
    }
}