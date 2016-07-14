using IdentityServer3.Core;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripCompany.Repository;
using TripCompany.Repository.Entities;
using IdentityServer3.Core.Extensions;
using System.Security.Claims;
using Microsoft.Owin;
using IdentityServer3.Core.Services;

namespace TripCompany.IdentityServer.Services
{
    public class CustomUserService : UserServiceBase
    {
        // Authenticate against local user store.
        public override Task AuthenticateLocalAsync(IdentityServer3.Core.Models.LocalAuthenticationContext context)
        {
            using (var userRepository = new UserRepository())
            {
                // get user from repository
                var user = userRepository.GetUser(context.UserName, context.Password);

                if (user == null)
                {
                    context.AuthenticateResult = new AuthenticateResult("Invalid credentials");
                    return Task.FromResult(0);
                }

                context.AuthenticateResult = new AuthenticateResult(
                    user.Subject,
                    user.UserClaims.First(c => c.ClaimType == Constants.ClaimTypes.GivenName).ClaimValue);

                return Task.FromResult(0);
            }
        }

        public override Task GetProfileDataAsync(IdentityServer3.Core.Models.ProfileDataRequestContext context)
        {
            using (var userRepository = new UserRepository())
            {
                // find the user
                var user = userRepository.GetUser(context.Subject.GetSubjectId());

                // add subject as claim
                var claims = new List<Claim>
                {
                    new Claim(Constants.ClaimTypes.Subject, user.Subject),
                };

                // add the other UserClaims
                claims.AddRange(user.UserClaims.Select<UserClaim, Claim>(
                    uc => new Claim(uc.ClaimType, uc.ClaimValue)));

                // only return the requested claims
                if (!context.AllClaimsRequested)
                {
                    claims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();
                }

                // set the issued claims - these are the ones that were requested, if available
                context.IssuedClaims = claims;

                return Task.FromResult(0);
            }
        }

        public override Task IsActiveAsync(IdentityServer3.Core.Models.IsActiveContext context)
        {
            using (var userRepository = new UserRepository())
            {
                if (context.Subject == null)
                {
                    throw new ArgumentNullException("subject");
                }

                var user = userRepository.GetUser(context.Subject.GetSubjectId());

                // set whether or not the user is active
                context.IsActive = (user != null) && user.IsActive;

                return Task.FromResult(0);
            }
        }
    }
}
