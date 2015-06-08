﻿using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using Org.XmlUnit.Builder;
using Org.XmlUnit.Constraints;
using Syringe.Core;
using Syringe.Core.Xml.Writer;

namespace Syringe.Tests.Unit.Xml
{
	public class TestCaseWriterTests
	{
		public static string XmlExamplesFolder = "Syringe.Tests.Unit.Xml.XmlExamples.Writer.";

		[Test]
		public void write_should_add_repeat_attribute()
		{
			// Arrange
			string expectedXml = TestHelpers.ReadEmbeddedFile("repeat.xml", XmlExamplesFolder);

			CaseCollection caseCollection = CreateCaseCollection();
			caseCollection.Repeat = 10;
			TestCaseWriter xmlWriter = CreateTestCaseWriter();

			// Act
			string actualXml = xmlWriter.Write(caseCollection);

			// Assert
			Assert.That(Input.FromString(actualXml), CompareConstraint.IsIdenticalTo(Input.FromString(expectedXml)));
		}

		[Test]
		public void write_should_add_all_case_attributes()
		{
			// Arrange
			string expectedXml = TestHelpers.ReadEmbeddedFile("all-attributes.xml", XmlExamplesFolder);

			var testCase = new Case()
			{
				Id = 1,
				ShortDescription = "short description",
				LongDescription = "long description",
				Url = "http://myserver",
				Method = "post",
				PostType = "text/xml",
				VerifyResponseCode = HttpStatusCode.Accepted,
				ErrorMessage = "my error message",
				LogRequest = true,
				LogResponse = true,
				Sleep = 3,
			};
			CaseCollection caseCollection = CreateCaseCollection(testCase);
			TestCaseWriter xmlWriter = CreateTestCaseWriter();

			// Act
			string actualXml = xmlWriter.Write(caseCollection);

			// Assert
			Assert.That(Input.FromString(actualXml), CompareConstraint.IsIdenticalTo(Input.FromString(expectedXml)));			
		}

		[Test]
		[Ignore]
		public void write_should_add_header_key_and_values_with_cdata_encoding_when_value_has_html_entities()
		{
			// Arrange
			string expectedXml = TestHelpers.ReadEmbeddedFile("headers.xml", XmlExamplesFolder);

			var testCase = new Case() { Id = 1 };
			testCase.AddHeader("key1", "value1");
			testCase.AddHeader("key2", "some <marvellous> HTML &&&&.");

			CaseCollection caseCollection = CreateCaseCollection(testCase);
			TestCaseWriter xmlWriter = CreateTestCaseWriter();

			// Act
			string actualXml = xmlWriter.Write(caseCollection);

			// Assert
			Assert.That(Input.FromString(actualXml), CompareConstraint.IsIdenticalTo(Input.FromString(expectedXml)));
		}

		[Test]
		public void write_should_add_postbody_with_cdata()
		{
			// Arrange
			string expectedXml = TestHelpers.ReadEmbeddedFile("postbody.xml", XmlExamplesFolder);

			var testCase = new Case() {Id = 1};
			testCase.PostBody = "username=corey&password=welcome&myhtml=<body></body>";
			CaseCollection caseCollection = CreateCaseCollection(testCase);
			TestCaseWriter xmlWriter = CreateTestCaseWriter();

			// Act
			string actualXml = xmlWriter.Write(caseCollection);

			// Assert
			Assert.That(Input.FromString(actualXml), CompareConstraint.IsIdenticalTo(Input.FromString(expectedXml)));
		}

		[Test]
		[Ignore]
		public void write_should_add_parsedresponses()
		{
			// Arrange
			string expectedXml = TestHelpers.ReadEmbeddedFile("todo", XmlExamplesFolder);

			CaseCollection caseCollection = CreateCaseCollection();
			TestCaseWriter xmlWriter = CreateTestCaseWriter();

			// Act
			string actualXml = xmlWriter.Write(caseCollection);

			// Assert
			Assert.That(Input.FromString(actualXml), CompareConstraint.IsIdenticalTo(Input.FromString(expectedXml)));
		}

		[Test]
		[Ignore]
		public void write_should_add_verifications()
		{
			// Arrange
			string expectedXml = TestHelpers.ReadEmbeddedFile("file", XmlExamplesFolder);

			CaseCollection caseCollection = CreateCaseCollection();
			TestCaseWriter xmlWriter = CreateTestCaseWriter();

			// Act
			string actualXml = xmlWriter.Write(caseCollection);

			// Assert
			Assert.That(Input.FromString(actualXml), CompareConstraint.IsIdenticalTo(Input.FromString(expectedXml)));
		}

		private TestCaseWriter CreateTestCaseWriter()
		{
			return new TestCaseWriter();
		}

		private CaseCollection CreateCaseCollection()
		{
			return new CaseCollection();
		}

		private CaseCollection CreateCaseCollection(Case testCase)
		{
			var caseCollection = new CaseCollection();
			var list = new List<Case>();
			list.Add(testCase);

			caseCollection.TestCases = list;

			return caseCollection;
		}
	}
}
