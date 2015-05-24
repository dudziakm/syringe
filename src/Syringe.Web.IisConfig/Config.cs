using System;
using System.IO;
using IisConfiguration;
using IisConfiguration.Configuration;

namespace Syringe.Web.IisConfig
{
    public class Config : EnvironmentalConfig
    {
	    public override string WebRoot
	    {
		    get
		    {
				// TODO: update IisConfiguration
			    string webdir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..")).FullName;
				return base.WebRoot.Replace("{src-path}", webdir);
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