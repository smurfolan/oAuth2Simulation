using System.Collections.Generic;
using System.Web.Mvc;

namespace BoongalooCompany.IdentityServer.Models
{
    // If the user uses external login, this is the model we are using in order to collect additional information from him before getting him logged in.
    public class CompleteAdditionalInformationModel
    {
        public string ExternalProvider { get; set; }
        public string Role { get; set; }

        public List<SelectListItem> Roles { get; set; }
        public CompleteAdditionalInformationModel()
        {
            Roles = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "Free version", Value = "FreeUser"},
                new SelectListItem() { Text = "Plus version", Value = "PayingUser"}
            };
        }
    }
}
