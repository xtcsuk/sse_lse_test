using System;
using System.Configuration;

namespace Asos.CodeTest
{
    public class AppSettings : IAppSettings
    {
        public bool IsFailoverModeEnabled
        {
            get { return Convert.ToBoolean(ConfigurationManager.AppSettings["IsFailoverModeEnabled"]); }
        }
    }
}