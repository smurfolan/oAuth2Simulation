using System.Web.Http;
using System.Web.Http.Cors;
using Boongaloo.DTO.Applicant;

namespace Boongaloo.API.Controllers
{
    [Authorize]
    [EnableCors("https://localhost:44316", "*", "GET, POST, DELETE")]
    public class UserDataController : ApiController
    {
        public UserData GetUserBySubjectAsync(string userSubject)
        {
            return null;
        }
    }
}
