using IdentityServer3.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripCompany.IdentityServer.Config;
using TripGallery;

namespace TripCompany.IdentityServer
{
    // example of a custom cors policy service
    public class CustomCorsPolicyService : ICorsPolicyService
    {
        public Task<bool> IsOriginAllowedAsync(string origin)
        {
            // use this method to check the origin passed in against
            // the origins you want to allow

            // eg, for Angular:
             
            // return Task.FromResult<bool>(
            //    (origin.ToLowerInvariant()
            //    == Constants.TripGalleryAngular.ToLowerInvariant()));

            // ... or for custom URI's, or by running through the 
            // redirectURI's of the defined clients, or ...

            // Clients.Get().Select(c => c.RedirectUris) ...

            return Task.FromResult<bool>(true);
        }
    }
}
