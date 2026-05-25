using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalQuest.Models._2020
{
    public class VersionResponse
    {
        // will get this enum at some point (I have been awake for so long right now askjhfgksajdf)
        public int VersionStatus { get; set; } = 0;
        public string NextUpdate { get; set; } = "2099-01-01T00:00:00Z";
    }
}
