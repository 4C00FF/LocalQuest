using QuerryNetworking.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalQuest.Controllers.ServiceControllers
{
    public class Accounts : ClientRequest
    {
        [Get("/account/bulk")]
        public List<Models.Modern.Profile> BulkProfile()
        {
            return new List<Models.Modern.Profile>()
            {
                new Models.Modern.Profile()
                {
                    accountId = long.Parse(Config.GetString("AccountId")),
                    displayName = Config.GetString("DisplayName"),
                    username = Config.GetString("Username"),
                    profileImage = Config.GetString("PFP")
                }
            };
        }

        [Get("/account/me")]
        public Models.Modern.Profile Me()
        {
            return new Models.Modern.Profile()
            {
                accountId = long.Parse(Config.GetString("AccountId")),
                displayName = Config.GetString("DisplayName"),
                username = Config.GetString("Username"),
                profileImage = Config.GetString("PFP")
            };
        }
    }
}
