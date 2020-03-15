using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace STP.Identity.Application
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Address(),
                new IdentityResources.Phone()
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("identity","API Identity"),
                new ApiResource("markets", "API Market"),
                new ApiResource("datafeed", "API Datafeed"),
                new ApiResource("profiles", "API Profile"),
                new ApiResource("realtime", "API Realtime"),
            };
        }

        public static class Scopes
        {
            public const string Identity = "identity";
            public const string Markets = "markets";
            public const string Datafeed = "datafeed";
            public const string Profiles = "profiles";
            public const string Realtime = "realtime";
            public const string Internal = "internal";
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "IOS",
                    ClientName = "IOS",

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowOfflineAccess = true,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },


                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess,

                        Scopes.Identity,
                        Scopes.Markets,
                        Scopes.Datafeed,
                        Scopes.Profiles,
                        Scopes.Realtime
                    }
                },

                new Client
                {
                    ClientId = "Android",
                    ClientName = "Android",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowOfflineAccess = true,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess,

                        Scopes.Identity,
                        Scopes.Markets,
                        Scopes.Datafeed,
                        Scopes.Profiles,
                        Scopes.Realtime
                    }
                },

                new Client
                {
                    ClientId = "WEB",
                    ClientName = "Market.WEB",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowOfflineAccess = true,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess,

                        Scopes.Identity,
                        Scopes.Markets,
                        Scopes.Datafeed,
                        Scopes.Profiles,
                        Scopes.Realtime
                    }
                },

                new Client
                {
                    ClientId = "Swagger",
                    ClientName = "Swagger",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowOfflineAccess = true,
                    
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess,

                        Scopes.Identity,
                        Scopes.Markets,
                        Scopes.Datafeed,
                        Scopes.Profiles,
                        Scopes.Realtime
                    }
                },

                new Client
                {
                    ClientId = "Inner",
                    ClientName = "Inner",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    RequireClientSecret = false,
                    AllowOfflineAccess = true,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Phone,

                        Scopes.Identity,
                        Scopes.Markets,
                        Scopes.Datafeed,
                        Scopes.Profiles
                    }
                }
            };
        }
    }
}