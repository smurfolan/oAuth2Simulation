using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using IdentityServer3.Core.Extensions;
using TripCompany.IdentityServer.Models;
using TripCompany.Repository;
using TripCompany.Repository.Entities;

namespace TripCompany.IdentityServer.Controllers
{
    public class CompleteAdditionalInformationController : Controller
    {
        // GET: CompleteAdditionalInformation
        public async Task<ActionResult> Index(string provider)
        {
            // we're only allowed here when we have a partial sign-in
            var ctx = Request.GetOwinContext();
            var partialSignInUser = await ctx.Environment.GetIdentityServerPartialLoginAsync();
            if (partialSignInUser == null)
            {
                return View("No partially signed-in user found.");
            }

            return View(new CompleteAdditionalInformationModel() {ExternalProvider = provider});
        }

        [HttpPost]
        public async Task<ActionResult> Index(CompleteAdditionalInformationModel model)
        {
            // we're only allowed here when we have a partial sign-in
            var ctx = Request.GetOwinContext();
            var partialSignInUser = await ctx.Environment.GetIdentityServerPartialLoginAsync();
            if (partialSignInUser == null)
            {
                return View("No partially signed-in user found.");
            }

            if (ModelState.IsValid)
            {
                using (var userRepository = new UserRepository())
                {
                    // create a user in our user store, including claims & google as
                    // an external login.

                    // create a new account
                    var newUser = new User();
                    newUser.Subject = Guid.NewGuid().ToString();
                    newUser.IsActive = true;

                    // add the external identity provider as login provider
                    // => external_provider_user_id contains the id/key
                    // TODO: Need to be able to dynamically determine what is the LoginProvider
                    newUser.UserLogins.Add(new UserLogin()
                    {
                        Subject = newUser.Subject,
                        LoginProvider = model.ExternalProvider.ToLower(),
                        ProviderKey = partialSignInUser.Claims.First(c => c.Type == "external_provider_user_id").Value
                    });

                    // create e-mail claim
                    newUser.UserClaims.Add(new UserClaim()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Subject = newUser.Subject,
                        ClaimType = IdentityServer3.Core.Constants.ClaimTypes.Email,
                        ClaimValue = partialSignInUser.Claims.First(
                          c => c.Type == IdentityServer3.Core.Constants.ClaimTypes.Email).Value
                    });

                    // create claims from the model
                    newUser.UserClaims.Add(new UserClaim()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Subject = newUser.Subject,
                        ClaimType = IdentityServer3.Core.Constants.ClaimTypes.GivenName,
                        ClaimValue = partialSignInUser.Claims.First(c => c.Type == "given_name").Value
                    });
                    newUser.UserClaims.Add(new UserClaim()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Subject = newUser.Subject,
                        ClaimType = IdentityServer3.Core.Constants.ClaimTypes.FamilyName,
                        ClaimValue = partialSignInUser.Claims.First(c => c.Type == "family_name").Value
                    });

                    // we could use the access token to obtain user info from linkedin
                    newUser.UserClaims.Add(new UserClaim()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Subject = newUser.Subject,
                        ClaimType = IdentityServer3.Core.Constants.ClaimTypes.AccessTokenHash,
                        ClaimValue = partialSignInUser.Claims.First(c => c.Type == "urn:linkedin:accesstoken").Value
                    });

                    newUser.UserClaims.Add(new UserClaim()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Subject = newUser.Subject,
                        ClaimType = "role",
                        ClaimValue = model.Role
                    });

                    // add the user             
                    userRepository.AddUser(newUser);

                    // continue where we left off                
                    return Redirect(await ctx.Environment.GetPartialLoginResumeUrlAsync());
                }
            }

            return View();
        }
    }
}