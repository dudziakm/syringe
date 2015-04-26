using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NUnit.Framework;
using Syringe.Core;
using Syringe.Core.Xml;

namespace Syringe.Tests.Unit.Xml
{
    public class LegacyTestCaseReaderTests : TestCaseReaderTests
	{
        protected override string XmlExamplesFolder
        {
            get { return "Syringe.Tests.Unit.Xml.LegacyXmlExamples."; }
        }

        protected override ITestCaseReader GetTestCaseReader(TextReader textReader)
        {
			return new LegacyTestCaseReader(textReader);
        }

		private string GetInvalidXml(string attributeName = "verifypositive")
		{
			const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
		            <testcases>
						<case
							id=""1""
							description1=""short description""
							{attributeName}=""\<SELECT\>somequerystring=a&anotherquerystring=b&nbsp;""
						/>
					</testcases>";
			return xml.Replace("{attributeName}", attributeName);
		}

		[Test]
		public void Read_should_parse_description_attributes_when_numbers_are_omitted()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			xml = xml.Replace("description1=", "description=");
			var stringReader = new StringReader(xml);
			var testCaseReader = GetTestCaseReader(stringReader);

			// Act
			CaseCollection testCollection = testCaseReader.Read();

			// Assert
			Case testcase = testCollection.TestCases.First();

			Assert.That(testcase.ShortDescription, Is.EqualTo("short description"));
			Assert.That(testcase.LongDescription, Is.EqualTo("long description"));
		}

        [Test]
        public void Read_should_cleanse_invalid_xml()
        {
            // Arrange
            string xml = ReadEmbeddedFile("invalid-xml.xml");
            var stringReader = new StringReader(xml);
			var testCaseReader = GetTestCaseReader(stringReader);

            // Act + Assert
            testCaseReader.Read();
        }

        [Test]
        public void Read_should_parse_empty_addheader()
        {
            // Arrange
            string xml = GetSingleCaseExample();
            xml = xml.Replace("mykey: 12345|bar: foo|emptyvalue:|Cookie: referer=harrispilton.com", "");
            var stringReader = new StringReader(xml);
			var testCaseReader = GetTestCaseReader(stringReader);

            // Act
            CaseCollection testCollection = testCaseReader.Read();

            // Assert
            Case testcase = testCollection.TestCases.First();
            Assert.That(testcase.Headers.Count, Is.EqualTo(0));
        }

        [Test]
        public void Read_should_parse_single_addheader_attribute()
        {
            // Arrange
            string xml = GetSingleCaseExample();
            xml = xml.Replace("mykey: 12345|bar: foo|emptyvalue:|Cookie: referer=harrispilton.com", "User-Agent: Mozilla/5.0");
            var stringReader = new StringReader(xml);
			var testCaseReader = GetTestCaseReader(stringReader);

            // Act
            CaseCollection testCollection = testCaseReader.Read();

            // Assert
            Case testcase = testCollection.TestCases.First();
            Assert.That(testcase.Headers.Count, Is.EqualTo(1));
            Assert.That(testcase.Headers[0].Key, Is.EqualTo("User-Agent"));
            Assert.That(testcase.Headers[0].Value, Is.EqualTo("Mozilla/5.0"));
        }

        [Test]
        public void GetNumberedAttributes_should_return_attributes_ordered_numerically()
        {
            // Arrange
            string xml = ReadEmbeddedFile("ordered-attributes.xml");
            XDocument document = XDocument.Parse(xml);
            var firstTestCase = document.Root.Elements().First(x => x.Name.LocalName == "case");

			var testCaseReader = new LegacyTestCaseReader(new StringReader(xml));

            // Act
			List<RegexItem> descriptions = testCaseReader.GetNumberedAttributes(firstTestCase, "parseresponse");

            // Assert
			Assert.That(descriptions[0].Description, Is.EqualTo("parseresponse0"));
			Assert.That(descriptions[0].Regex, Is.EqualTo("parse 0"));

			Assert.That(descriptions[1].Description, Is.EqualTo("parseresponse1"));
			Assert.That(descriptions[1].Regex, Is.EqualTo("parse 1"));

			Assert.That(descriptions[2].Description, Is.EqualTo("parseresponse2"));
			Assert.That(descriptions[2].Regex, Is.EqualTo("parse 2"));

			Assert.That(descriptions[3].Description, Is.EqualTo("parseresponse12"));
			Assert.That(descriptions[3].Regex, Is.EqualTo("parse 12"));
        }

		[Test]
		public void ReEncodeAttributeValues_should_change_invalid_xml_into_valid_xml()
		{
			// Arrange
			string invalidXml = GetInvalidXml();
			string nbsp = XmlConvert.EncodeName("&nbsp;");

			// Act
			string validXml = LegacyTestCaseReader.ReEncodeAttributeValues(invalidXml);

			// Assert
			XDocument document = XDocument.Parse(validXml);
			XAttribute verifyPositive = document.Root.Elements().First(x => x.Name.LocalName == "case").Attribute("verifypositive");
			Assert.That(verifyPositive.Value, Is.EqualTo("<SELECT>somequerystring=a&anotherquerystring=b" + nbsp));
		}

		[Test]
		public void ReEncodeAttributeValues_should_ignore_empty_attribute_value()
		{
			// Arrange
			string xml = GetInvalidXml();
			xml = xml.Replace(@"verifypositive=""\<SELECT\>somequerystring=a&anotherquerystring=b&nbsp;""", "verifypositive=\"\"");

			// Act
			string validXml = LegacyTestCaseReader.ReEncodeAttributeValues(xml);

			// Assert
			XDocument document = XDocument.Parse(validXml);
			XAttribute verifyPositive = document.Root.Elements().First(x => x.Name.LocalName == "case").Attribute("verifypositive");
			Assert.That(verifyPositive.Value, Is.EqualTo(""));
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
			string invalidXml = GetInvalidXml(attributeName);
			string nbsp = XmlConvert.EncodeName("&nbsp;");

			// Act
			string validXml = LegacyTestCaseReader.ReEncodeAttributeValues(invalidXml);

			// Assert
			XDocument document = XDocument.Parse(validXml);
			XAttribute verifyPositive = document.Root.Elements().First(x => x.Name.LocalName == "case").Attribute(attributeName);
			Assert.That(verifyPositive.Value, Is.EqualTo("<SELECT>somequerystring=a&anotherquerystring=b" + nbsp));
		}
	}
}
