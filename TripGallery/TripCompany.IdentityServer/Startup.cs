using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services.Default;
using IdentityServer3.Core.Services.InMemory;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.Core.Services;
using Microsoft.Owin.Security.Google;
using Owin.Security.Providers.LinkedIn;
using TripCompany.IdentityServer.Config;
using TripCompany.IdentityServer.Services;

namespace TripCompany.IdentityServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/identity", idsrvApp =>
            {
                var corsPolicyService = new DefaultCorsPolicyService()
                {
                    AllowAll = true
                };

                var idServerServiceFactory = new IdentityServerServiceFactory()
                                .UseInMemoryClients(Clients.Get())
                                .UseInMemoryScopes(Scopes.Get());

                var defaultViewServiceOptions = new DefaultViewServiceOptions
                {
                    CacheViews = false
                };

                idServerServiceFactory.ConfigureDefaultViewService(defaultViewServiceOptions);

                idServerServiceFactory.CorsPolicyService = new
                    Registration<IdentityServer3.Core.Services.ICorsPolicyService>(corsPolicyService);

                // use custom user service
                var customUserService = new CustomUserService();
                idServerServiceFactory.UserService = new Registration<IUserService>(resolver => customUserService);

                var options = new IdentityServerOptions
                {
                    Factory = idServerServiceFactory,
                    SiteName = "TripCompany Security Token Service",
                    SigningCertificate = LoadCertificate(),
                    IssuerUri = TripGallery.Constants.TripGalleryIssuerUri,
                    PublicOrigin = TripGallery.Constants.TripGallerySTSOrigin,

                    AuthenticationOptions = new AuthenticationOptions()
                    {
                        EnablePostSignOutAutoRedirect = true,
                        PostSignOutAutoRedirectDelay = 2,
                        // Provide link to other(additional) web page in order to be able to login (Registration, Reset password etc.)
                        LoginPageLinks = new List<LoginPageLink>()
                        {
                            new LoginPageLink()
                            {
                                Type = "createaccount"/*Should be unique*/,
                                Text = "Crate a new account",
                                Href = "~/createuseraccount"
                            }
                        },
                        IdentityProviders = this.ConfigureIdentityProviders
                    },
                    CspOptions = new CspOptions()
                    {
                        Enabled = false
                        // once available, leave enabled at true and use:
                        // FrameSrc = "https://localhost:44318 https://localhost:44316"
                        // or
                        // FrameSrc = "*" for all URIs
                    }

                };

                idsrvApp.UseIdentityServer(options);
            });
        }

        X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2(
                string.Format(@"{0}\certificates\idsrv3test.pfx",
                AppDomain.CurrentDomain.BaseDirectory), "idsrv3test");
        }

        private void ConfigureIdentityProviders(IAppBuilder app, string signInAsType)
        {
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                AuthenticationType = "Google",
                Caption = "Sign-in with Google",
                SignInAsAuthenticationType = signInAsType,

                ClientId = "97756425623-pf0l0olj4d0bjiuaslg5cmd330g4bit9.apps.googleusercontent.com",
                ClientSecret = "y68P2OYWJtsb3yWl9rR_DsyS",
                Provider = new GoogleOAuth2AuthenticationProvider()
                {
                    OnAuthenticated = (context) =>
                    {
                        // TODO: Instead of using context.* for assignments, we could try to make a call to google's user info endpoint passing the access token we have
                        context.Identity.AddClaim(new System.Security.Claims.Claim(
                                    IdentityServer3.Core.Constants.ClaimTypes.GivenName, context.GivenName));

                        context.Identity.AddClaim(new System.Security.Claims.Claim(
                            IdentityServer3.Core.Constants.ClaimTypes.FamilyName, context.FamilyName));      
                        // since there's no roles in Google, we explicitly set it to 'FreeUser'                  
                        context.Identity.AddClaim(new System.Security.Claims.Claim(
                         IdentityServer3.Core.Constants.ClaimTypes.Role, "FreeUser"));

                        return Task.FromResult(0);
                    }
                }
            });

            app.UseLinkedInAuthentication(new LinkedInAuthenticationOptions()
            {
                AuthenticationType = "LinkedIn",
                Caption = "Sign-in with LinkedIn",
                SignInAsAuthenticationType = signInAsType,

                ClientId = "777k6ke7zh5851",
                ClientSecret = "3VuCX21r1AkXnGmd",
                Provider = new LinkedInAuthenticationProvider()
                {
                    OnAuthenticated = (context) =>
                    {
                        context.Identity.AddClaim(new System.Security.Claims.Claim(
                                    IdentityServer3.Core.Constants.ClaimTypes.GivenName, context.GivenName));

                        context.Identity.AddClaim(new System.Security.Claims.Claim(
                            IdentityServer3.Core.Constants.ClaimTypes.FamilyName, context.FamilyName));

                        // since there's no roles in LinkedIn, we explicitly set it to 'FreeUser'                  
                        context.Identity.AddClaim(new System.Security.Claims.Claim(
                         IdentityServer3.Core.Constants.ClaimTypes.Role, "FreeUser"));
                        
                        return Task.FromResult(0);
                    }
                }

            });
        }
    }
}
