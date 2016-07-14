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
                        // Provide link to other web page in order to be able to login (Registration, Reset password etc.)
                        LoginPageLinks = new List<LoginPageLink>()
                        {
                            new LoginPageLink()
                            {
                                Type = "createaccount"/*Should be unique*/,
                                Text = "Crate a new account",
                                Href = "~/createuseraccount"
                            }
                        }
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
    }
}
