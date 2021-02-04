using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Configuration
{
    public class AppMongoSettingsOptions
    {
        public const string ApplicationSettingName = "AppMongoSettings";
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}
