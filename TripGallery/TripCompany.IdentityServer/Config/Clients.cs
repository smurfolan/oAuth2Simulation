using IdentityServer3.Core.Models;
using System.Collections.Generic;

namespace TripCompany.IdentityServer.Config
{
    public static class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new[]
             { 
                 new Client 
                {
                     ClientId = "tripgalleryclientcredentials",
                     ClientName = "Trip Gallery (Client Credentials)",
                     Flow = Flows.ClientCredentials,  
                     AllowAccessToAllScopes = true,

                    ClientSecrets = new List<Secret>()
                    {
                        new Secret(TripGallery.Constants.TripGalleryClientSecret.Sha256())
                    }                  
                }
                ,
                new Client 
                {
                    ClientId = "tripgalleryauthcode",
                    ClientName = "Trip Gallery (Authorization Code)",
                    Flow = Flows.AuthorizationCode, 
                    AllowAccessToAllScopes = true,

                    // redirect = URI of our callback controller in the MVC application
                    RedirectUris = new List<string>
                    { 
                        TripGallery.Constants.TripGalleryMVCSTSCallback 
                    },           

                     ClientSecrets = new List<Secret>()
                    {
                        new Secret(TripGallery.Constants.TripGalleryClientSecret.Sha256())
                    }                    
                } ,
                new Client 
                {
                    ClientId = "tripgalleryimplicit",
                    ClientName = "Trip Gallery (Implicit)",
                    Flow = Flows.Implicit, 
                    AllowAccessToAllScopes = true,

                    IdentityTokenLifetime = 10,
                    AccessTokenLifetime = 120,

                    // redirect = URI of the Angular application
                    RedirectUris = new List<string>
                    { 
                        TripGallery.Constants.TripGalleryAngular + "callback.html" 
                    }            
                }
                ,
                new Client 
                {
                    ClientId = "tripgalleryropc",
                    ClientName = "Trip Gallery (Resource Owner Password Credentials)",
                    Flow = Flows.ResourceOwner, 
                    AllowAccessToAllScopes = true,

                    ClientSecrets = new List<Secret>()
                    {
                        new Secret(TripGallery.Constants.TripGalleryClientSecret.Sha256())
                    }                    
                },
                new Client 
                {
                    ClientId = "tripgalleryhybrid",
                    ClientName = "Trip Gallery (Hybrid)",
                    Flow = Flows.Hybrid, 
                    AllowAccessToAllScopes = true,

                    IdentityTokenLifetime = 10,
                    AccessTokenLifetime = 120,



                    // redirect = URI of the MVC application
                    RedirectUris = new List<string>
                    { 
                        TripGallery.Constants.TripGalleryMVC 
                    },
                    
                    // Needed when requesting refresh tokens
                    ClientSecrets = new List<Secret>()
                    {
                        new Secret(TripGallery.Constants.TripGalleryClientSecret.Sha256())
                    }                   
                }  

             };
        }
    }
}
