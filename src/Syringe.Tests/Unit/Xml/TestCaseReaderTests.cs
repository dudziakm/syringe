using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Xml.Linq;
using NUnit.Framework;
using Syringe.Core;
using Syringe.Core.Exceptions;
using Syringe.Core.Xml.LegacyConverter;

namespace Syringe.Tests.Unit.Xml
{
	public class TestCaseReaderTests
	{
	    protected virtual string XmlExamplesFolder
	    {
	        get { return "Syringe.Tests.Unit.Xml.XmlExamples."; }
	    }

        protected virtual ITestCaseReader GetReader()
        {
            return new TestCaseReader();
        }

		protected string ReadEmbeddedFile(string file)
		{
            string path = string.Format("{0}{1}", XmlExamplesFolder, file);

			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
			if (stream == null)
				throw new InvalidOperationException(string.Format("Unable to find '{0}' as an embedded resource", path));

			string result = "";
			using (StreamReader reader = new StreamReader(stream))
			{
				result = reader.ReadToEnd();
			}

			return result;
		}

        protected string GetSingleCaseExample()
		{
			return ReadEmbeddedFile("single-case.xml");
		}

        protected string GetFullExample()
		{
			return ReadEmbeddedFile("full.xml");
		}

		[Test]
		public void Read_should_throw_exception_when_testcases_element_is_missing()
		{
			// Arrange
			string xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?><something></something>";
			var stringReader = new StringReader(xml);
			var testCaseReader = GetReader();

			// Act + Assert
			Assert.Throws<TestCaseException>(() => testCaseReader.Read(stringReader));
		}

		[Test]
		public void Read_should_parse_repeat_attribute()
		{
			// Arrange
			string xml = GetFullExample();
			var stringReader = new StringReader(xml);;
			var testCaseReader = GetReader();

			// Act
			TestCaseCollection testCollection = testCaseReader.Read(stringReader);

			// Assert
			Assert.That(testCollection.Repeat, Is.EqualTo(10));
		}

		[Test]
		public void Read_should_parse_test_vars()
		{
			// Arrange
			string xml = GetFullExample();
			var stringReader = new StringReader(xml);
			var testCaseReader = GetReader();

			// Act
			TestCaseCollection testCollection = testCaseReader.Read(stringReader);

			// Assert
			Assert.That(testCollection.Variables.Count, Is.EqualTo(4));
			Assert.That(testCollection.Variables["LOGIN_URL"], Is.EqualTo("http://myserver/login.php"));
			Assert.That(testCollection.Variables["LOGIN1"], Is.EqualTo("bob"));
			Assert.That(testCollection.Variables["PASSWD1"], Is.EqualTo("sponge"));
			Assert.That(testCollection.Variables["SUCCESSFULL_TEST_TEXT"], Is.EqualTo("Welcome Bob"));
		}

		[Test]
		public void Read_should_parse_ids_and_order_numerically()
		{
			// Arrange
			string xml = GetFullExample();
			var stringReader = new StringReader(xml);
			var testCaseReader = GetReader();

			// Act
			TestCaseCollection testCollection = testCaseReader.Read(stringReader);

			// Assert
			List<TestCase> testCases = testCollection.TestCases.ToList();

			Assert.That(testCases[0].Id, Is.EqualTo(1));
			Assert.That(testCases[1].Id, Is.EqualTo(9));
			Assert.That(testCases[2].Id, Is.EqualTo(20));
			Assert.That(testCases[3].Id, Is.EqualTo(300));
		}

		[Test]
		public void Read_should_parse_method_attribute()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			var stringReader = new StringReader(xml);
			var testCaseReader = GetReader();

			// Act
			TestCaseCollection testCollection = testCaseReader.Read(stringReader);

			// Assert
			TestCase testcase = testCollection.TestCases.First();

			Assert.That(testcase.Method, Is.EqualTo("post"));
		}

		[Test]
		public void Read_should_default_method_property_to_get_when_it_doesnt_exist()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			xml = xml.Replace(@"method=""post""", "");

			var stringReader = new StringReader(xml);
			var testCaseReader = GetReader();

			// Act
			TestCaseCollection testCollection = testCaseReader.Read(stringReader);

			// Assert
			TestCase testcase = testCollection.TestCases.First();

			Assert.That(testcase.Method, Is.EqualTo("get"));
		}

		[Test]
		public void Read_should_parse_url_attribute()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			var stringReader = new StringReader(xml);
			var testCaseReader = GetReader();

			// Act
			TestCaseCollection testCollection = testCaseReader.Read(stringReader);

			// Assert
			TestCase testcase = testCollection.TestCases.First();
			Assert.That(testcase.Url, Is.EqualTo("http://myserver"));
		}

		[Test]
		public void Read_should_throw_exception_when_url_attribute_doesnt_exist()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			xml = xml.Replace(@"url=""http://myserver""", "");

			var stringReader = new StringReader(xml);
			var testCaseReader = GetReader();

			// Act + Assert
			Assert.Throws<TestCaseException>(() => testCaseReader.Read(stringReader));
		}

		[Test]
		public void Read_should_parse_postbody_attribute()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			var stringReader = new StringReader(xml);
			var testCaseReader = GetReader();

			// Act
			TestCaseCollection testCollection = testCaseReader.Read(stringReader);

			// Assert
			TestCase testcase = testCollection.TestCases.First();
			Assert.That(testcase.PostBody, Is.EqualTo("username=corey&password=welcome"));
		}

		[Test]
		public void Read_should_parse_errormessage_attribute()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			var stringReader = new StringReader(xml);
			var testCaseReader = GetReader();

			// Act
			TestCaseCollection testCollection = testCaseReader.Read(stringReader);

			// Assert
			TestCase testcase = testCollection.TestCases.First();
			Assert.That(testcase.ErrorMessage, Is.EqualTo("my error message"));
		}

		[Test]
		public void Read_should_parse_posttype_attribute()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			var stringReader = new StringReader(xml);
			var testCaseReader = GetReader();

			// Act
			TestCaseCollection testCollection = testCaseReader.Read(stringReader);

			// Assert
			TestCase testcase = testCollection.TestCases.First();
			Assert.That(testcase.PostType, Is.EqualTo("text/xml"));
		}

		[Test]
		public void Read_should_use_default_posttype_value_when_attribute_is_empty()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			xml = xml.Replace("posttype=\"text/xml\"", "");

			var stringReader = new StringReader(xml);
			var testCaseReader = GetReader();

			// Act
			TestCaseCollection testCollection = testCaseReader.Read(stringReader);

			// Assert
			TestCase testcase = testCollection.TestCases.First();
			Assert.That(testcase.PostType, Is.EqualTo("application/x-www-form-urlencoded"));
		}

		[Test]
		public void Read_should_parse_responsecode_attribute()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			var stringReader = new StringReader(xml);
			var testCaseReader = GetReader();
			var expectedCode = HttpStatusCode.NotFound;

			// Act
			TestCaseCollection testCollection = testCaseReader.Read(stringReader);

			// Assert
			TestCase testcase = testCollection.TestCases.First();
			Assert.That(testcase.VerifyResponseCode, Is.EqualTo(expectedCode));
		}

		[Test]
		public void Read_should_use_default_responsecode_value_when_attribute_is_empty()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			xml = xml.Replace("verifyresponsecode=\"404\"", "");

			var stringReader = new StringReader(xml);
			var testCaseReader = GetReader();
			var expectedCode = HttpStatusCode.OK;

			// Act
			TestCaseCollection testCollection = testCaseReader.Read(stringReader);

			// Assert
			TestCase testcase = testCollection.TestCases.First();
			Assert.That(testcase.VerifyResponseCode, Is.EqualTo(expectedCode));
		}

		[Test]
		public void Read_should_use_default_responsecode_value_when_attribute_is_invalid_code()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			xml = xml.Replace("verifyresponsecode=\"404\"", "verifyresponsecode=\"20000000\"");

			var stringReader = new StringReader(xml);
			var testCaseReader = GetReader();
			var expectedCode = HttpStatusCode.OK;

			// Act
			TestCaseCollection testCollection = testCaseReader.Read(stringReader);

			// Assert
			TestCase testcase = testCollection.TestCases.First();
			Assert.That(testcase.VerifyResponseCode, Is.EqualTo(expectedCode));
		}

		[Test]
		public void Read_should_parse_logrequest_attribute()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			var stringReader = new StringReader(xml);
			var testCaseReader = GetReader();

			// Act
			TestCaseCollection testCollection = testCaseReader.Read(stringReader);

			// Assert
			TestCase testcase = testCollection.TestCases.First();
			Assert.That(testcase.LogRequest, Is.False);
		}

		[Test]
		public void Read_should_set_logrequest_to_true_value_when_attribute_is_missing()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			xml = xml.Replace("logrequest=\"no\"", "");

			var stringReader = new StringReader(xml);
			var testCaseReader = GetReader();

			// Act
			TestCaseCollection testCollection = testCaseReader.Read(stringReader);

			// Assert
			TestCase testcase = testCollection.TestCases.First();
			Assert.That(testcase.LogRequest, Is.True);
		}

		[Test]
		public void Read_should_parse_logresponse_attribute()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			var stringReader = new StringReader(xml);
			var testCaseReader = GetReader();

			// Act
			TestCaseCollection testCollection = testCaseReader.Read(stringReader);

			// Assert
			TestCase testcase = testCollection.TestCases.First();
			Assert.That(testcase.LogResponse, Is.False);
		}

		[Test]
		public void Read_should_set_logresponse_to_true_value_when_attribute_is_missing()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			xml = xml.Replace("logresponse=\"no\"", "");

			var stringReader = new StringReader(xml);
			var testCaseReader = GetReader();

			// Act
			TestCaseCollection testCollection = testCaseReader.Read(stringReader);

			// Assert
			TestCase testcase = testCollection.TestCases.First();
			Assert.That(testcase.LogResponse, Is.True);
		}

		[Test]
		public void Read_should_parse_sleep_attribute()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			var stringReader = new StringReader(xml);
			var testCaseReader = GetReader();

			// Act
			TestCaseCollection testCollection = testCaseReader.Read(stringReader);

			// Assert
			TestCase testcase = testCollection.TestCases.First();
			Assert.That(testcase.Sleep, Is.EqualTo(3));
		}

		[Test]
		public void Read_should_set_default_sleep_to_zero_when_attribute_is_missing()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			xml = xml.Replace("sleep=\"3\"", "");

			var stringReader = new StringReader(xml);
			var testCaseReader = GetReader();

			// Act
			TestCaseCollection testCollection = testCaseReader.Read(stringReader);

			// Assert
			TestCase testcase = testCollection.TestCases.First();
			Assert.That(testcase.Sleep, Is.EqualTo(0));
		}

		[Test]
		public void Read_should_parse_addheader_attribute()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			var stringReader = new StringReader(xml);
			var testCaseReader = GetReader();

			// Act
			TestCaseCollection testCollection = testCaseReader.Read(stringReader);

			// Assert
			TestCase testcase = testCollection.TestCases.First();
			Assert.That(testcase.AddHeader[0].Key, Is.EqualTo("mykey"));
			Assert.That(testcase.AddHeader[0].Value, Is.EqualTo("12345"));

			Assert.That(testcase.AddHeader[1].Key, Is.EqualTo("bar"));
			Assert.That(testcase.AddHeader[1].Value, Is.EqualTo("foo"));

			Assert.That(testcase.AddHeader[2].Key, Is.EqualTo("emptyvalue"));
			Assert.That(testcase.AddHeader[2].Value, Is.EqualTo(""));

			Assert.That(testcase.AddHeader[3].Key, Is.EqualTo("Cookie"));
			Assert.That(testcase.AddHeader[3].Value, Is.EqualTo("referer=harrispilton.com"));
		}

		[Test]
		public void Read_should_populate_parseresponse()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			var stringReader = new StringReader(xml);
			var testCaseReader = GetReader();

			// Act
			TestCaseCollection testCollection = testCaseReader.Read(stringReader);

			// Assert
			TestCase testcase = testCollection.TestCases.First();
            Assert.That(testcase.ParseResponses.Count, Is.EqualTo(3));
			Assert.That(testcase.ParseResponses[0].Value, Is.EqualTo("parse 1"));
            Assert.That(testcase.ParseResponses[1].Value, Is.EqualTo("parse 11"));
            Assert.That(testcase.ParseResponses[2].Value, Is.EqualTo("parse 99"));
		}

		[Test]
		public void Read_should_populate_verifypositive()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			var stringReader = new StringReader(xml);
			var testCaseReader = GetReader();

			// Act
			TestCaseCollection testCollection = testCaseReader.Read(stringReader);

			// Assert
			TestCase testcase = testCollection.TestCases.First();
            Assert.That(testcase.VerifyPositives.Count, Is.EqualTo(3));
            Assert.That(testcase.VerifyPositives[0].Value, Is.EqualTo("positive 1"));
            Assert.That(testcase.VerifyPositives[1].Value, Is.EqualTo("positive 22"));
            Assert.That(testcase.VerifyPositives[2].Value, Is.EqualTo("positive 99"));
		}

		[Test]
		public void Read_should_populate_verifynegative()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			var stringReader = new StringReader(xml);
			var testCaseReader = GetReader();

			// Act
			TestCaseCollection testCollection = testCaseReader.Read(stringReader);

			// Assert
			TestCase testcase = testCollection.TestCases.First();
            Assert.That(testcase.VerifyNegatives.Count, Is.EqualTo(3));
			Assert.That(testcase.VerifyNegatives[0].Value, Is.EqualTo("negative 1"));
            Assert.That(testcase.VerifyNegatives[1].Value, Is.EqualTo("negative 6"));
            Assert.That(testcase.VerifyNegatives[2].Value, Is.EqualTo("negative 66"));
		}

		[Test]
        [Ignore("TODO")]
        public void GetElementCollection_should_return_attributes_ordered_numerically()
		{
			// Arrange
			string xml = ReadEmbeddedFile("ordered-attributes.xml");
			XDocument document = XDocument.Parse(xml);
			var firstTestCase = document.Root.Elements().First(x => x.Name.LocalName == "case");

		    var testCaseReader = new TestCaseReader();

			// Act
            List<NumberedAttribute> descriptions = testCaseReader.GetElementCollection(firstTestCase, "description", "descriptions", "description");

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
