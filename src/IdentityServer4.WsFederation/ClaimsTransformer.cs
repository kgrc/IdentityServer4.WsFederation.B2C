using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace IdentityServer4.WsFederation
{
    public class ClaimsTransformer : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            // This will run every time Authenticate is called so its better to create a new Principal
            //Ratheesh: Just adding a custom claim 'timestamp' to the outgoing claim
            //Edit this to add more claims as RP needs.
            var transformed = new ClaimsPrincipal();
            transformed.AddIdentities(principal.Identities);
            transformed.AddIdentity(new ClaimsIdentity(new Claim[]
            {
                new Claim("Timestamp", DateTime.Now.ToString())
            }));
            return Task.FromResult(transformed);
        }
    }
}