using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using Boongaloo.DTO;

namespace Boongaloo.MVCClient.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult PostAuthorization()
        {
            // Need to determine if the user is available or not in our system. If yes, redirect to a separate view, else go to the bottom one.
            // That is the place where he has to add additional information and get stored into the DB.
            

            var userRole = ClaimsPrincipal.Current.Claims.First(claim => claim.Type == IdentityModel.JwtClaimTypes.Role).Value;

            if(userRole == RolesEnum.JobApplicant.ToString())
                return RedirectToAction("Index", "ApplicantRelatedInfo", new { area = "Application" });

            else if(userRole == RolesEnum.Recruiter.ToString())
                return RedirectToAction("Index", "RecruiterRelatedInfo", new { area = "Recruitment" });

            return null;
        }
    }
}