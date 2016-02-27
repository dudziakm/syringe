using System;
using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using Org.XmlUnit.Builder;
using Org.XmlUnit.Constraints;
using Syringe.Core.TestCases;
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
				Id = Guid.Empty,
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
		public void write_should_add_header_key_and_values_with_cdata_encoding_when_value_has_html_entities()
		{
			// Arrange
			string expectedXml = TestHelpers.ReadEmbeddedFile("headers.xml", XmlExamplesFolder);

			var testCase = new Case() { Id = Guid.Empty };
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

			var testCase = new Case() {Id = Guid.Empty};
			testCase.PostBody = "username=corey&password=welcome&myhtml=<body></body>";
			CaseCollection caseCollection = CreateCaseCollection(testCase);
			TestCaseWriter xmlWriter = CreateTestCaseWriter();

			// Act
			string actualXml = xmlWriter.Write(caseCollection);

			// Assert
			Assert.That(Input.FromString(actualXml), CompareConstraint.IsIdenticalTo(Input.FromString(expectedXml)));
		}

		[Test]
		public void write_should_add_parseresponses()
		{
			// Arrange
			string expectedXml = TestHelpers.ReadEmbeddedFile("parseresponses.xml", XmlExamplesFolder);

			var testCase = new Case() { Id = Guid.Empty };
			testCase.ParseResponses.Add(new ParseResponseItem("1", "here is (.*?) regex"));
			testCase.ParseResponses.Add(new ParseResponseItem("2", "plain text"));
			testCase.ParseResponses.Add(new ParseResponseItem("3", "This is encoded <test> &."));

			CaseCollection caseCollection = CreateCaseCollection(testCase);
			TestCaseWriter xmlWriter = CreateTestCaseWriter();

			// Act
			string actualXml = xmlWriter.Write(caseCollection);

			// Assert
			Assert.That(Input.FromString(actualXml), CompareConstraint.IsIdenticalTo(Input.FromString(expectedXml)));
		}

		[Test]
		public void write_should_add_verifications()
		{
			// Arrange
			string expectedXml = TestHelpers.ReadEmbeddedFile("verifications.xml", XmlExamplesFolder);

			var testCase = new Case() { Id = Guid.Empty };
			testCase.VerifyPositives.Add(new VerificationItem("p90-1", "regex1", VerifyType.Positive));
			testCase.VerifyPositives.Add(new VerificationItem("p90-2", "regex2", VerifyType.Positive));
			testCase.VerifyPositives.Add(new VerificationItem("p90-3", "this 3rd positive needs CDATA as it has <html> & stuff in it (.*)", VerifyType.Positive));

			testCase.VerifyNegatives.Add(new VerificationItem("negev1", "regex1", VerifyType.Negative));
			testCase.VerifyNegatives.Add(new VerificationItem("negev2", "regex2", VerifyType.Negative));
			testCase.VerifyNegatives.Add(new VerificationItem("negev3", "this 3rd negative needs CDATA as it has <html> & stuff in it (.*)", VerifyType.Negative));

			CaseCollection caseCollection = CreateCaseCollection(testCase);
			TestCaseWriter xmlWriter = CreateTestCaseWriter();

			// Act
			string actualXml = xmlWriter.Write(caseCollection);

			// Assert
			Assert.That(Input.FromString(actualXml), CompareConstraint.IsIdenticalTo(Input.FromString(expectedXml)));
		}

		[Test]
		public void write_should_write_large_files()
		{
			// Arrange
			string expectedXml = TestHelpers.ReadEmbeddedFile("large-file.xml", XmlExamplesFolder);

			var caseCollection = new CaseCollection();
			var list = new List<Case>();		

			for (int i = 0; i < 100; i++)
			{
				var testCase = new Case()
				{
					Id = Guid.Empty,
					ShortDescription = "short description" +i,
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

				list.Add(testCase);
			}

			caseCollection.TestCases = list;
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
