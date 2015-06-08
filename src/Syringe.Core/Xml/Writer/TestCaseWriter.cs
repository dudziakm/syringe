using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Syringe.Core.Xml.Writer
{
	public class TestCaseWriter
	{
		public string Write(CaseCollection caseCollection)
		{
			var stringBuilder = new StringBuilder();
			using (var stringWriter = new Utf8StringWriter(stringBuilder))
			{
				XmlWriter xmlWriter = new XmlTextWriter(stringWriter);

				XElement testCasesElement = new XElement("testcases");
				testCasesElement.Add(new XAttribute("repeat", caseCollection.Repeat.ToString()));

				foreach (Case testCase in caseCollection.TestCases)
				{
					XElement headersElement = GetHeadersElement(testCase);
					XElement postbodyElement = GetPostBodyElement(testCase);

					XElement caseElement = GetCaseElement(testCase);
					caseElement.Add(headersElement);
					caseElement.Add(postbodyElement);

					testCasesElement.Add(caseElement);
				}

				XDocument doc = new XDocument(testCasesElement);
				doc.WriteTo(xmlWriter);
			}

			return stringBuilder.ToString();
		}

		private XElement GetCaseElement(Case testCase)
		{
			XElement element = new XElement("case");

			element.Add(new XAttribute("id", testCase.Id));
			element.Add(new XAttribute("shortdescription", testCase.ShortDescription ?? ""));
			element.Add(new XAttribute("longdescription", testCase.LongDescription ?? ""));
			element.Add(new XAttribute("url", testCase.Url ?? ""));
			element.Add(new XAttribute("method", testCase.Method ?? ""));
			element.Add(new XAttribute("posttype", testCase.PostType ?? ""));
			element.Add(new XAttribute("verifyresponsecode", (int)testCase.VerifyResponseCode));
			element.Add(new XAttribute("errormessage", testCase.ErrorMessage ?? ""));
			element.Add(new XAttribute("logrequest", testCase.LogRequest));
			element.Add(new XAttribute("logresponse", testCase.LogResponse));
			element.Add(new XAttribute("sleep", testCase.Sleep));

			return element;
		}

		private XElement GetHeadersElement(Case testCase)
		{
			XElement headerElement = new XElement("headers");

			foreach (KeyValuePair<string, string> keyValuePair in testCase.Headers)
			{
				if (!string.IsNullOrEmpty(keyValuePair.Key))
				{
					XElement element = new XElement("header");
					element.Add(new XAttribute("name", keyValuePair.Key));

					if (!string.IsNullOrEmpty(keyValuePair.Value))
					{
						if (keyValuePair.Value.Contains("&") ||
						    keyValuePair.Value.Contains("<") || keyValuePair.Value.Contains(">"))
						{
							element.Add(new XCData(keyValuePair.Value));
						}
						else
						{
							element.Value = keyValuePair.Value;
						}
					}

					headerElement.Add(element);
				}
			}

			return headerElement;
		}

		private XElement GetPostBodyElement(Case testCase)
		{
			XElement postBodyElement = new XElement("postbody");
			postBodyElement.Add(new XCData(testCase.PostBody));

			return postBodyElement;
		}
	}
}