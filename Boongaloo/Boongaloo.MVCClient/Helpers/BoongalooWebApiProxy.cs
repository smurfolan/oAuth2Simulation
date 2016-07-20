using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;
using Boongaloo.DTO.Applicant;

namespace Boongaloo.MVCClient.Helpers
{
    // TODO: See how to ensure API result from HttpResponseMessage
    public class BoongalooWebApiProxy
    {
        private HttpClient _client;

        public BoongalooWebApiProxy()
        {
            this._client = BoongalooHttpClient.GetClient();
        }

        public async Task<UserData> GetUserBySubjectAsync(string subject)
        {
            var userDataResponse = await this._client
                .GetAsync("api/UserData/GetUserBySubject?userSubject=" + subject); //httpClient.GetAsync("api/trips/" + tripId.ToString() + "/pictures").ConfigureAwait(false);


            return await Task.FromResult(new UserData());
        }
    }
}