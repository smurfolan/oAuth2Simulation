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
            var userRole = ClaimsPrincipal.Current.Claims.First(claim => claim.Type == IdentityModel.JwtClaimTypes.Role).Value;

            if(userRole == RolesEnum.JobApplicant.ToString())
                return RedirectToAction("Index", "ApplicantRelatedInfo", new { area = "Application" });

            else if(userRole == RolesEnum.Recruiter.ToString())
                return RedirectToAction("Index", "RecruiterRelatedInfo", new { area = "Recruitment" });

            return null;
        }
    }
}