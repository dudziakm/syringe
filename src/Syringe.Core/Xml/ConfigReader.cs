using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ConfigurationException = Syringe.Core.Exceptions.ConfigurationException;

namespace Syringe.Core.Xml
{
	public class ConfigReader
	{
		public Config Read(TextReader textReader)
		{
			var config = new Config();
			XDocument doc = XDocument.Load(textReader);

			// Check for <root>
			XElement rootElement = doc.Elements().FirstOrDefault(i => i.Name.LocalName == "root");
			if (rootElement == null)
				throw new ConfigurationException("<root> node is missing from the config file.");

			// Fill the known properties
			config.BaseUrl = XmlHelper.GetOptionalElementValue(rootElement, "baseurl");
			config.Proxy = XmlHelper.GetOptionalElementValue(rootElement, "proxy");
			config.Useragent = XmlHelper.GetOptionalElementValue(rootElement, "useragent");
			config.Httpauth = XmlHelper.GetOptionalElementValue(rootElement, "httpauth");
			config.Comment = XmlHelper.GetOptionalElementValue(rootElement, "comment");
			config.Timeout = XmlHelper.ElementAsInt(rootElement, "timeout");
			config.GlobalTimeout = XmlHelper.ElementAsInt(rootElement, "globaltimeout");

			var logType = LogType.None;
			string httpLog = XmlHelper.GetOptionalElementValue(rootElement, "globalhttplog");
			if (!string.IsNullOrEmpty(httpLog))
			{
				Enum.TryParse(httpLog, true, out logType);	
			}
			config.GlobalHttpLog = logType;

			// All elements get stored in the variables, for custom variables.
			foreach (XElement element in rootElement.Elements())
			{
				if (!config.Variables.ContainsKey(element.Name.LocalName))
				{
					config.Variables.Add(element.Name.LocalName, element.Value);
				}
			}

			return config;
		}

		public static Config Load(string configFilename)
		{
			using (var stringReader = new StringReader(File.ReadAllText(configFilename)))
			{
				var configReader = new ConfigReader();
				return configReader.Read(stringReader);
			}
		}
	}
}
