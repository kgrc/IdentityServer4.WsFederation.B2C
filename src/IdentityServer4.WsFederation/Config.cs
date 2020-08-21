using IdentityModel;
using IdentityServer4.Models;
using System.Collections.Generic;
using static IdentityServer4.IdentityServerConstants;
using IdentityServer4.WsFederation.Stores;
using System.IdentityModel.Claims;
using System.IdentityModel.Tokens;

namespace IdentityServer4.WsFederation
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new[]
            {
                new IdentityResources.OpenId(),
                new IdentityResource("profile", new[] { JwtClaimTypes.Name, JwtClaimTypes.Email })
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new[]
            {
                new ApiResource("api1", "Some API 1"),
                new ApiResource("api2", "Some API 2")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                new Client
                {
                    ClientId = "urn:owinrp",
                    ProtocolType = ProtocolTypes.WsFederation,

                    RedirectUris = { "http://localhost:10313/" },
                    FrontChannelLogoutUri = "http://localhost:10313/home/signoutcleanup",
                    IdentityTokenLifetime = 36000,

                    AllowedScopes = { "openid", "profile" }
                },
                new Client
                {
                    ClientId = "urn:aspnetcorerp",
                    ProtocolType = ProtocolTypes.WsFederation,

                    RedirectUris = { "http://localhost:10314/" },
                    FrontChannelLogoutUri = "http://localhost:10314/account/signoutcleanup",
                    IdentityTokenLifetime = 36000,

                    AllowedScopes = { "openid", "profile" }
                }
            };
        }

        public static List<RelyingParty> GetRelyingParties()
        {
            return new List<RelyingParty> {
            new RelyingParty {
                // Same as ClientId. Used to link config
                Realm = "urn:aspnetcorerp",

                // SAML 1.1 token type required by SharePoint
                TokenType = WsFederationConstants.TokenTypes.Saml11TokenProfile11,

                // Transform claim types from oidc standard to xml types
                // Only mapped claims will be returned for SAML 1.1 tokens
                ClaimMapping = new Dictionary<string, string> {
                    {JwtClaimTypes.Subject, ClaimTypes.NameIdentifier},
                    {JwtClaimTypes.Name, ClaimTypes.Name},
                    {JwtClaimTypes.Email, ClaimTypes.Email},
                    {JwtClaimTypes.GivenName, ClaimTypes.GivenName}
                },

                // Defaults
                DigestAlgorithm = SecurityAlgorithms.Sha256Digest,
                SignatureAlgorithm = SecurityAlgorithms.RsaSha256Signature,
                SamlNameIdentifierFormat = WsFederationConstants.SamlNameIdentifierFormats.UnspecifiedString
            }
        };
        }
    }
}