using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using NUnit.Framework;
using Syringe.Core.Exceptions;
using Syringe.Core.Xml;

namespace Syringe.Tests.Unit.Xml
{
	public class TestCaseReaderTests
	{
		private string GetExample(string file)
		{
			string path = string.Format("Syringe.Tests.Unit.Xml.TestCaseExamples.{0}", file);

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

		private string GetSingleCaseExample()
		{
			return GetExample("single-case.xml");
		}

		private string GetFullExample()
		{
			return GetExample("full-example.xml");
		}

		[Test]
		public void Read_should_throw_exception_when_testcases_element_is_missing()
		{
			// Arrange
			string xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?><something></something>";
			var stringReader = new StringReader(xml);
			var testCaseReader = new TestCaseReader();

			// Act + Assert
			Assert.Throws<TestCaseException>(() => testCaseReader.Read(stringReader));
		}

		[Test]
		public void Read_should_cleanse_invalid_xml()
		{
			// Arrange
			string xml = GetExample("invalid-xml.xml");
			var stringReader = new StringReader(xml);
			var testCaseReader = new TestCaseReader();

			// Act + Assert
			testCaseReader.Read(stringReader);
		}

		[Test]
		public void Read_should_parse_repeat_attribute()
		{
			// Arrange
			string xml = GetExample("full-example.xml");
			var stringReader = new StringReader(xml);;
			var testCaseReader = new TestCaseReader();

			// Act
			TestCaseContainer container = testCaseReader.Read(stringReader);

			// Assert
			Assert.That(container.Repeat, Is.EqualTo(10));
		}

		[Test]
		public void Read_should_parse_test_vars()
		{
			// Arrange
			string xml = GetFullExample();
			var stringReader = new StringReader(xml);
			var testCaseReader = new TestCaseReader();

			// Act
			TestCaseContainer container = testCaseReader.Read(stringReader);

			// Assert
			Assert.That(container.Variables.Count, Is.EqualTo(4));
			Assert.That(container.Variables["LOGIN_URL"], Is.EqualTo("http://myserver/login.php"));
			Assert.That(container.Variables["LOGIN1"], Is.EqualTo("bob"));
			Assert.That(container.Variables["PASSWD1"], Is.EqualTo("sponge"));
			Assert.That(container.Variables["SUCCESSFULL_TEST_TEXT"], Is.EqualTo("Welcome Bob"));
		}

		[Test]
		public void Read_should_parse_ids_and_order_numerically()
		{
			// Arrange
			string xml = GetFullExample();
			var stringReader = new StringReader(xml);
			var testCaseReader = new TestCaseReader();

			// Act
			TestCaseContainer container = testCaseReader.Read(stringReader);

			// Assert
			List<TestCase> testCases = container.TestCases.ToList();

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
			var testCaseReader = new TestCaseReader();

			// Act
			TestCaseContainer container = testCaseReader.Read(stringReader);

			// Assert
			TestCase testcase = container.TestCases.First();

			Assert.That(testcase.Method, Is.EqualTo("post"));
		}

		[Test]
		public void Read_should_default_method_property_to_get_when_it_doesnt_exist()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			xml = xml.Replace(@"method=""post""", "");

			var stringReader = new StringReader(xml);
			var testCaseReader = new TestCaseReader();

			// Act
			TestCaseContainer container = testCaseReader.Read(stringReader);

			// Assert
			TestCase testcase = container.TestCases.First();

			Assert.That(testcase.Method, Is.EqualTo("get"));
		}

		[Test]
		public void Read_should_parse_url_attribute()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			var stringReader = new StringReader(xml);
			var testCaseReader = new TestCaseReader();

			// Act
			TestCaseContainer container = testCaseReader.Read(stringReader);

			// Assert
			TestCase testcase = container.TestCases.First();
			Assert.That(testcase.Url, Is.EqualTo("http://server"));
		}

		[Test]
		public void Read_should_throw_exception_when_url_attribute_doesnt_exist()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			xml = xml.Replace(@"url=""http://server""", "");

			var stringReader = new StringReader(xml);
			var testCaseReader = new TestCaseReader();

			// Act + Assert
			Assert.Throws<TestCaseException>(() => testCaseReader.Read(stringReader));
		}

		[Test]
		public void Read_should_parse_postbody_attribute()
		{
			// Arrange
			string xml = GetSingleCaseExample();
			var stringReader = new StringReader(xml);
			var testCaseReader = new TestCaseReader();

			// Act
			TestCaseContainer container = testCaseReader.Read(stringReader);

			// Assert
			TestCase testcase = container.TestCases.First();
			Assert.That(testcase.PostBody, Is.EqualTo("username=corey&password=welcome"));
		}

		[Test]
		public void GetOrderedAttributes_should_return_attributes_ordered_numerically()
		{
			// Arrange
			string xml = GetExample("multiple-attributes.xml");
			XDocument document = XDocument.Parse(xml);
			var firstTestCase = document.Root.Elements().First(x => x.Name.LocalName == "case");

			var testCaseReader = new TestCaseReader();

			// Act
			List<string> descriptions = testCaseReader.GetOrderedAttributes(firstTestCase, "description");

			// Assert
			Assert.That(descriptions[0], Is.EqualTo("description 1"));
			Assert.That(descriptions[1], Is.EqualTo("description 2"));
			Assert.That(descriptions[2], Is.EqualTo("description 99"));
		}
	}
}
