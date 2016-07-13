using System;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Helpers;
using IdentityModel.Client;


[assembly: OwinStartup(typeof(TripGallery.MVCClient.Startup))]
namespace TripGallery.MVCClient
{
    public class Startup
    {

        public void Configuration(IAppBuilder app)
        {

            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            AntiForgeryConfig.UniqueClaimTypeIdentifier = 
                IdentityModel.JwtClaimTypes.Name;

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies",
                // How much the cookie will remain valid from the point it is created.
                ExpireTimeSpan = new TimeSpan(0, 30, 0),
                // Tells the middleware to issue a new cookie with a new expiration time after half of the expiration window has passed.
                SlidingExpiration = true
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {

                ClientId = "tripgalleryhybrid",
                Authority = Constants.TripGallerySTS,
                RedirectUri = Constants.TripGalleryMVC,
                SignInAsAuthenticationType = "Cookies",
                ResponseType = "code id_token token",
                Scope = "openid profile address gallerymanagement roles offline_access",
                // identity_token lifetime won't be used, but the expiration options of the Authentication ticket will be used.
                UseTokenLifetime = false, 

                Notifications = new OpenIdConnectAuthenticationNotifications()
                {
                    SecurityTokenValidated = async n =>
                    {
                        Helpers.TokenHelper.DecodeAndWrite(n.ProtocolMessage.IdToken);
                        Helpers.TokenHelper.DecodeAndWrite(n.ProtocolMessage.AccessToken);

                        // CLAIMS TRANSFORMATION
                        var givenNameClaim = n.AuthenticationTicket
                            .Identity.FindFirst(IdentityModel.JwtClaimTypes.GivenName);

                        var familyNameClaim = n.AuthenticationTicket
                            .Identity.FindFirst(IdentityModel.JwtClaimTypes.FamilyName);

                        var subClaim = n.AuthenticationTicket
                            .Identity.FindFirst(IdentityModel.JwtClaimTypes.Subject);

                        var roleClaim = n.AuthenticationTicket
                            .Identity.FindFirst(IdentityModel.JwtClaimTypes.Role);

                        // create a new claims, issuer + sub as unique identifier
                        var nameClaim = new Claim(IdentityModel.JwtClaimTypes.Name,
                                    Constants.TripGalleryIssuerUri + subClaim.Value);

                        var newClaimsIdentity = new ClaimsIdentity(
                           n.AuthenticationTicket.Identity.AuthenticationType,
                           IdentityModel.JwtClaimTypes.Name,
                           IdentityModel.JwtClaimTypes.Role);

                        if (nameClaim != null)
                        {
                            newClaimsIdentity.AddClaim(nameClaim);
                        }

                        if (givenNameClaim != null)
                        {
                            newClaimsIdentity.AddClaim(givenNameClaim);
                        }

                        if (familyNameClaim != null)
                        {
                            newClaimsIdentity.AddClaim(familyNameClaim);
                        }

                        if (roleClaim != null)
                        {
                            newClaimsIdentity.AddClaim(roleClaim);
                        }

                        // request a refresh token
                        var tokenClientForRefreshToekn = new TokenClient(
                            Constants.TripGallerySTSTokenEndpoint,
                            "tripgalleryhybrid",
                            Constants.TripGalleryClientSecret);

                        var refreshResponse = await tokenClientForRefreshToekn
                            .RequestAuthorizationCodeAsync(
                                n.ProtocolMessage.Code,
                                Constants.TripGalleryMVC);

                        var expirationDateAsRoundtripString = DateTime
                            .SpecifyKind(DateTime.UtcNow.AddSeconds(refreshResponse.ExpiresIn)
                                , DateTimeKind.Utc).ToString("o");

                        newClaimsIdentity.AddClaim(new Claim("refresh_token", refreshResponse.RefreshToken));
                        newClaimsIdentity.AddClaim(new Claim("access_token", refreshResponse.AccessToken));
                        newClaimsIdentity.AddClaim(new Claim("expires_at", expirationDateAsRoundtripString));

                        // create a new authentication ticket, overwriting the old one.
                        n.AuthenticationTicket = new AuthenticationTicket(
                                                 newClaimsIdentity,
                                                 n.AuthenticationTicket.Properties);

                        await Task.FromResult(0);
                    }
                }
            });

        }
    }
}
