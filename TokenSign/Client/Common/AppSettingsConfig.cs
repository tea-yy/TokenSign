using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Client.Common
{
    public class AppSettingsConfig
    {
        public static string GetTokenApi
        {
            get
            {
                return AppSettingValue();
            }
        }

        public static string StaffId
        {
            get
            {
                return AppSettingValue();
            }
        }

        private static string AppSettingValue([CallerMemberName] string key = null)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
