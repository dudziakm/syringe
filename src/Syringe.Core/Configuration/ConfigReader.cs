using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Syringe.Core.Xml;
using ConfigurationException = Syringe.Core.Exceptions.ConfigurationException;

namespace Syringe.Core.Configuration
{
	public class ConfigReader : IConfigReader
	{
		public Config Read(TextReader textReader)
		{
			var config = new Config();
			XDocument doc = XDocument.Load(textReader);

			// Check for <root>
			XElement configElement = doc.Elements().FirstOrDefault(i => i.Name.LocalName == "Config");
			if (configElement == null)
				throw new ConfigurationException("<Config> node is missing from the config file.");

			// Fill the known properties
			config.BaseUrl = XmlHelper.GetOptionalElementValue(configElement, "baseurl");
			config.Proxy = XmlHelper.GetOptionalElementValue(configElement, "proxy");
			config.Useragent = XmlHelper.GetOptionalElementValue(configElement, "useragent");
			config.Httpauth = XmlHelper.GetOptionalElementValue(configElement, "httpauth");
			config.Comment = XmlHelper.GetOptionalElementValue(configElement, "comment");
			config.Timeout = XmlHelper.ElementAsInt(configElement, "timeout");
			config.GlobalTimeout = XmlHelper.ElementAsInt(configElement, "globaltimeout");

			var logType = LogType.None;
			string httpLog = XmlHelper.GetOptionalElementValue(configElement, "globalhttplog");
			if (!string.IsNullOrEmpty(httpLog))
			{
				Enum.TryParse(httpLog, true, out logType);
			}
			config.GlobalHttpLog = logType;

			// All elements get stored in the variables, for custom variables.
			foreach (XElement element in configElement.Elements().Where(x => x.Name.LocalName == "Variables").Descendants())
			{
				config.Variables.Add(new Variable(element.Attribute("name").Value, element.Value));
			}

			return config;
		}
	}
}
