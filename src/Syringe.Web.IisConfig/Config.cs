using System;
using System.IO;
using IisConfiguration;
using IisConfiguration.Configuration;

namespace Syringe.Web.IisConfig
{
    public class Config : EnvironmentalConfig
    {
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
                return 8085;
            }
        }
    }
}