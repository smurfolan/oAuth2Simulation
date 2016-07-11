using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using IdentityModel.Client;

namespace TripGallery.MVCClient.Helpers
{
    public static class TripGalleryHttpClient
    {

        public static HttpClient GetClient()
        { 
            HttpClient client = new HttpClient();

            var accessToken = RequestAccessTokenAuthorizationCode();
            if (accessToken != null)
            {
                client.SetBearerToken(accessToken);
            }           

            // client credentials flow
            //var accessToken = RequestAccessTokenClientCredentials();
            //client.SetBearerToken(accessToken);
           
            client.BaseAddress = new Uri(Constants.TripGalleryAPI);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        private static string RequestAccessTokenAuthorizationCode()
        {
            // did we store the token before?
            var cookie = HttpContext.Current.Request.Cookies.Get("TripGalleryCookie");
            if (cookie != null && cookie["access_token"] != null)
            {
                return cookie["access_token"];
            }

            // no token found - request one

            // we'll pass through the URI we want to return to as state
            var state = HttpContext.Current.Request.Url.OriginalString;

            var authorizeRequest = new IdentityModel.Client.AuthorizeRequest(
                TripGallery.Constants.TripGallerySTSAuthorizationEndpoint);

            var url = authorizeRequest.CreateAuthorizeUrl("tripgalleryauthcode", "code", "gallerymanagement",
                TripGallery.Constants.TripGalleryMVCSTSCallback, state);
 
            HttpContext.Current.Response.Redirect(url);

            return null;
        }

 
        private static string RequestAccessTokenClientCredentials()
        {
            // did we store the token before?
            var cookie = HttpContext.Current.Request.Cookies.Get("TripGalleryCookie");
            if (cookie != null && cookie["access_token"] != null)
            {
                return cookie["access_token"];
            }
 
            // no token found - get one

            // create an oAuth2 Client
            var oAuth2Client = new TokenClient(
                      TripGallery.Constants.TripGallerySTSTokenEndpoint,
                      "tripgalleryclientcredentials",
                      TripGallery.Constants.TripGalleryClientSecret);

            // ask for a token, containing the gallerymanagement scope
            var tokenResponse = oAuth2Client.RequestClientCredentialsAsync("gallerymanagement").Result;

            // decode & write out the token, so we can see what's in it
            TokenHelper.DecodeAndWrite(tokenResponse.AccessToken);
            
            // we save the token in a cookie for use later on
            HttpContext.Current.Response.Cookies["TripGalleryCookie"]["access_token"] = tokenResponse.AccessToken;

            // return the token
            return tokenResponse.AccessToken;             
        }
         
    }
}