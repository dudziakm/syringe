using System.IO;
using NUnit.Framework;
using Syringe.Core;
using Syringe.Core.Configuration;
using Syringe.Core.Exceptions;
using Syringe.Core.Extensions;

namespace Syringe.Tests.Unit.Configuration
{
    public class ConfigReaderTests
    {
		[Test]
		public void Read_should_throw_exception_when_root_element_is_missing()
		{
			// Arrange
			string xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?><something></something>";
			var stringReader = new StringReader(xml);
			var configReader = new ConfigReader();

			// Act + Assert
			Assert.Throws<ConfigurationException>(() => configReader.Read(stringReader));
		}

		[Test]
		public void Read_should_populate_known_properties()
	    {
		    // Arrange
			var configReader = new ConfigReader();
			string xml = GetConfigXml();
			StringReader stringReader = new StringReader(xml);

			// Act
			Config config = configReader.Read(stringReader);

			// Assert
			Assert.That(config.Variables, Is.Not.Null);
			Assert.That(config.BaseUrl, Is.EqualTo("http://www.google.com"));
			Assert.That(config.Proxy, Is.EqualTo("my proxy"));
			Assert.That(config.Useragent, Is.EqualTo("AOL IE 6"));
			Assert.That(config.Httpauth, Is.EqualTo("http://username:password@127.0.0.1:8080"));
			Assert.That(config.GlobalHttpLog, Is.EqualTo(LogType.OnFail));
			Assert.That(config.Comment, Is.EqualTo("My config comment"));
			Assert.That(config.Timeout, Is.EqualTo(44));
			Assert.That(config.GlobalTimeout, Is.EqualTo(88));
	    }

	    [Test]
	    public void Read_should_populate_custom_variables()
	    {
		    // Arrange
		    var configReader = new ConfigReader();
		    string xml = GetConfigXmlWithCustomVariables();
		    StringReader stringReader = new StringReader(xml);

		    // Act
		    Config config = configReader.Read(stringReader);

		    // Assert
			Assert.That(config.Variables.ByName("baseurl"), Is.EqualTo("http://www.google.com"));
			Assert.That(config.Variables.ByName("baseurl1"), Is.EqualTo("http://www.bing.com"));
			Assert.That(config.Variables.ByName("baseurl88"), Is.EqualTo("http://www.yahoo.com"));
			Assert.That(config.Variables.ByName("myvariable"), Is.EqualTo("http://www.dogpile.com"));
	    }

	    private string GetConfigXml()
	    {
		    return @"<?xml version=""1.0"" encoding=""utf-8""?>
					<Config>
						<baseurl>http://www.google.com</baseurl>
						<proxy>my proxy</proxy>
						<useragent>AOL IE 6</useragent>
						<httpauth>http://username:password@127.0.0.1:8080</httpauth>
						<globalhttplog>onfail</globalhttplog>
						<comment>My config comment</comment>
						<timeout>44</timeout>
						<globaltimeout>88</globaltimeout>
					</Config>";
	    }

		private string GetConfigXmlWithCustomVariables()
		{
			return @"<?xml version=""1.0"" encoding=""utf-8""?>
						<Config>
							<proxy>my proxy</proxy>
							<Variables>
								<Variable name=""baseurl"">http://www.google.com</Variable>
								<Variable name=""baseurl"">http://www.google2.com</Variable>
								<Variable name=""baseurl1"">http://www.bing.com</Variable>
								<Variable name=""baseurl88"">http://www.yahoo.com</Variable>
								<Variable name=""myvariable"">http://www.dogpile.com</Variable>
							</Variables>
						</Config>";
		}
    }
}
