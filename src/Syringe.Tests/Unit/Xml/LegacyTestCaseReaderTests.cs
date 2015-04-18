using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Syringe.Core;
using Syringe.Core.Xml.LegacyConverter;

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
            List<NumberedAttribute> descriptions = testCaseReader.GetNumberedAttributes(firstTestCase, "description");

            // Assert
            Assert.That(descriptions[1].Name, Is.EqualTo("description"));
            Assert.That(descriptions[0].Index, Is.EqualTo(0));
            Assert.That(descriptions[0].Value, Is.EqualTo("description with no number"));

            Assert.That(descriptions[1].Name, Is.EqualTo("description"));
            Assert.That(descriptions[1].Index, Is.EqualTo(1));
            Assert.That(descriptions[1].Value, Is.EqualTo("description 1"));

            Assert.That(descriptions[2].Name, Is.EqualTo("description"));
            Assert.That(descriptions[2].Index, Is.EqualTo(2));
            Assert.That(descriptions[2].Value, Is.EqualTo("description 2"));

            Assert.That(descriptions[3].Name, Is.EqualTo("description"));
            Assert.That(descriptions[3].Index, Is.EqualTo(99));
            Assert.That(descriptions[3].Value, Is.EqualTo("description 99"));
        }
	}
}
