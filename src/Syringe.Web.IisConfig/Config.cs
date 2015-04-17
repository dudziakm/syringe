using System;
using System.IO;
using IisConfiguration;

namespace Syringe.Web.IisConfig
{
    public class Config : EnvironmentalConfig
    {
        public override string WebRoot
        {
            get
            {
                //TODO: fix this by the time it goes out for deploy
                string target = System.Environment.CurrentDirectory;
                target = target.Substring(0, target.IndexOf("src\\Syringe.Web.IisConfig", StringComparison.InvariantCultureIgnoreCase));
                target += "src\\Syringe.Web";

                return Path.GetFullPath(target);
            }
        }

        public string SiteName
        {
            get
            {
                return "Syringe.Web";
            }
        }

        public int PortNumber
        {
            get
            {
                return 1231;
            }
        }
    }
}