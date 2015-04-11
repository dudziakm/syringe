using System.IO;
using NUnit.Framework;
using Syringe.Core.Xml;

namespace Syringe.Tests.Unit
{
    public class ConfigReaderTests
    {
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
			Assert.That(config.GlobalHttpLog, Is.EqualTo("onfail"));
			Assert.That(config.Comment, Is.EqualTo("My config comment"));
			Assert.That(config.Timeout, Is.EqualTo("44"));
			Assert.That(config.GlobalTimeout, Is.EqualTo("88"));
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
			Assert.That(config.Variables["baseurl"], Is.EqualTo("http://www.google.com"));
		    Assert.That(config.Variables["baseurl1"], Is.EqualTo("http://www.bing.com"));
			Assert.That(config.Variables["baseurl88"], Is.EqualTo("http://www.yahoo.com"));
			Assert.That(config.Variables["myvariable"], Is.EqualTo("http://www.dogpile.com"));
	    }

	    private string GetConfigXml()
	    {
		    return @"<?xml version=""1.0"" encoding=""utf-8""?>
		            <root>
						<baseurl>http://www.google.com</baseurl>
						<proxy>my proxy</proxy>
						<useragent>AOL IE 6</useragent>
						<httpauth>http://username:password@127.0.0.1:8080</httpauth>
						<globalhttplog>onfail</globalhttplog>
						<comment>My config comment</comment>
						<timeout>44</timeout>
						<globaltimeout>88</globaltimeout>
					</root>";
	    }

		private string GetConfigXmlWithCustomVariables()
		{
			return @"<?xml version=""1.0"" encoding=""utf-8""?>
		            <root>
						<proxy>my proxy</proxy>
						<baseurl>http://www.google.com</baseurl>
						<baseurl1>http://www.bing.com</baseurl1>
						<baseurl88>http://www.yahoo.com</baseurl88>
						<myvariable>http://www.dogpile.com</myvariable>
					</root>";
		}
    }
}
