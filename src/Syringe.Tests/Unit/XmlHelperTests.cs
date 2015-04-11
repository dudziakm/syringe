using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Syringe.Core.Exceptions;
using Syringe.Core.Xml;

namespace Syringe.Tests.Unit
{
    public class XmlHelperTests
    {
		[Test]
		public void GetOptionalElementValue_should_return_element_value()
		{
			// Arrange
			string xml = GetBasicXml();
			var element = XElement.Parse(xml);

			// Act
			string actualValue = XmlHelper.GetOptionalElementValue(element, "baseurl");

			// Assert
			Assert.That(actualValue, Is.EqualTo("a value"));
		}

		[Test]
		public void GetOptionalElementValue_should_return_empty_string_when_element_does_not_exist()
		{
			// Arrange
			string xml = GetBasicXml();
			var element = XElement.Parse(xml);

			// Act
			string actualValue = XmlHelper.GetOptionalElementValue(element, "foobar");

			// Assert
			Assert.That(actualValue, Is.EqualTo(""));
		}

		[Test]
		public void GetRequiredElementValue_should_return_element_value()
		{
			// Arrange
			string xml = GetBasicXml();
			var element = XElement.Parse(xml);

			// Act
			string actualValue = XmlHelper.GetRequiredElementValue(element, "baseurl");

			// Assert
			Assert.That(actualValue, Is.EqualTo("a value"));
		}

		[Test]
	    public void GetRequiredElementValue_should_throw_exception_for_missing_element()
	    {
		    // Arrange
			string xml = GetBasicXml();
			var element = XElement.Parse(xml);

			// Act + Assert
			Assert.Throws<ConfigurationException>(() => XmlHelper.GetRequiredElementValue(element, "foobar"));
	    }

		[Test]
		public void ReEncodeAttributeValues_should_change_invalid_xml_into_valid_xml()
		{
			// Arrange
			string invalidXml = GetXmlWithAmpersands();

			// Act
			string validXml = XmlHelper.ReEncodeAttributeValues(invalidXml);

			// Assert
			XDocument document = XDocument.Parse(validXml);
			XAttribute verifyPositive = document.Root.Elements().First(x => x.Name.LocalName == "case").Attribute("verifypositive");
			Assert.That(verifyPositive.Value, Is.EqualTo("<SELECT>somequerystring=a&anotherquerystring=b"));
		}

		[Test]
		[TestCase("verifypositive")]
		[TestCase("verifypositive99")]
		[TestCase("verifynegative")]
		[TestCase("verifypositive88")]
		[TestCase("verifynextpositive")]
		[TestCase("verifynextnegative")]
		[TestCase("url")]
		[TestCase("addheader")]
		[TestCase("postbody")]
		public void ReEncodeAttributeValues_should_transform_all_known_webinject_attributes(string attributeName)
		{
			// Arrange
			string invalidXml = GetXmlWithAmpersands(attributeName);

			// Act
			string validXml = XmlHelper.ReEncodeAttributeValues(invalidXml);

			// Assert
			XDocument document = XDocument.Parse(validXml);
			XAttribute verifyPositive = document.Root.Elements().First(x => x.Name.LocalName == "case").Attribute(attributeName);
			Assert.That(verifyPositive.Value, Is.EqualTo("<SELECT>somequerystring=a&anotherquerystring=b"));
		}

	    private string GetBasicXml()
	    {
		    return @"<?xml version=""1.0"" encoding=""utf-8""?>
		            <root>
						<baseurl>a value</baseurl>
					</root>";
	    }

		private string GetXmlWithAmpersands(string attributeName = "verifypositive")
		{
			const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
		            <cases>
						<case
							id=""1""
							description1=""short description""
							{attributeName}=""\<SELECT\>somequerystring=a&anotherquerystring=b""
						/>
					</cases>";
			return  xml.Replace("{attributeName}", attributeName);
		}
    }
}
