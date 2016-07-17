using IdentityServer3.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoongalooCompany.IdentityServer.Config
{
    public static class Scopes
    {
        public static IEnumerable<Scope> Get()
        {
            return new List<Scope>
                { 
                    StandardScopes.OpenId,
                    StandardScopes.ProfileAlwaysInclude,
                    StandardScopes.Address, 
                    // Needed when we are working with refresh tokens
                    StandardScopes.OfflineAccess,                    
                    new Scope
                    { 
                        Name = "gallerymanagement",
                        DisplayName = "Gallery Management",
                        Description = "Allow the application to manage galleries on your behalf.",
                        Type = ScopeType.Resource,
                        // Once we ask for the gallery management scope, in the access token we receive,
                        // we will have a role claim.
                        Claims = new List<ScopeClaim>()
                        {
                            new ScopeClaim("role", false)
                        }
                    },
                    new Scope
                    { 
                        Name = "roles",
                        DisplayName = "Role(s)",
                        Description = "Allow the application to see your role(s).",
                        Type = ScopeType.Identity,
                        Claims = new List<ScopeClaim>()
                        {
                            new ScopeClaim("role", true)
                        }
                    }
                };
        }
    }
}
