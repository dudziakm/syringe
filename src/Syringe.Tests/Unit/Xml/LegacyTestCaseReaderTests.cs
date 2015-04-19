using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        protected override ITestCaseReader GetReader()
        {
            return new LegacyTestCaseReader();
        }

		[Test]
		public void Read_should_parse_description_attributes_when_numbers_are_omitted()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			xml = xml.Replace("description1=", "description=");
			var stringReader = new StringReader(xml);
			var testCaseReader = GetReader();

			// Act
			TestCaseCollection testCollection = testCaseReader.Read(stringReader);

			// Assert
			TestCase testcase = testCollection.TestCases.First();

			Assert.That(testcase.ShortDescription, Is.EqualTo("short description"));
			Assert.That(testcase.LongDescription, Is.EqualTo("long description"));
		}

        [Test]
        public void Read_should_cleanse_invalid_xml()
        {
            // Arrange
            string xml = ReadEmbeddedFile("invalid-xml.xml");
            var stringReader = new StringReader(xml);
            var testCaseReader = GetReader();

            // Act + Assert
            testCaseReader.Read(stringReader);
        }

        [Test]
        public void Read_should_parse_empty_addheader()
        {
            // Arrange
            string xml = GetSingleCaseExample();
            xml = xml.Replace("mykey: 12345|bar: foo|emptyvalue:|Cookie: referer=harrispilton.com", "");
            var stringReader = new StringReader(xml);
            var testCaseReader = GetReader();

            // Act
            TestCaseCollection testCollection = testCaseReader.Read(stringReader);

            // Assert
            TestCase testcase = testCollection.TestCases.First();
            Assert.That(testcase.AddHeader.Count, Is.EqualTo(0));
        }

        [Test]
        public void Read_should_parse_single_addheader_attribute()
        {
            // Arrange
            string xml = GetSingleCaseExample();
            xml = xml.Replace("mykey: 12345|bar: foo|emptyvalue:|Cookie: referer=harrispilton.com", "User-Agent: Mozilla/5.0");
            var stringReader = new StringReader(xml);
            var testCaseReader = GetReader();

            // Act
            TestCaseCollection testCollection = testCaseReader.Read(stringReader);

            // Assert
            TestCase testcase = testCollection.TestCases.First();
            Assert.That(testcase.AddHeader.Count, Is.EqualTo(1));
            Assert.That(testcase.AddHeader[0].Key, Is.EqualTo("User-Agent"));
            Assert.That(testcase.AddHeader[0].Value, Is.EqualTo("Mozilla/5.0"));
        }

        [Test]
        public void GetNumberedAttributes_should_return_attributes_ordered_numerically()
        {
            // Arrange
            string xml = ReadEmbeddedFile("ordered-attributes.xml");
            XDocument document = XDocument.Parse(xml);
            var firstTestCase = document.Root.Elements().First(x => x.Name.LocalName == "case");

            var testCaseReader = new LegacyTestCaseReader();

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
	}
}
