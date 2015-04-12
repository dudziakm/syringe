using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Syringe.Core.Exceptions;

namespace Syringe.Core.Xml
{
	public class TestCaseReader
	{
		public TestCaseContainer Read(TextReader textReader)
		{
			// Clean up any invalid XML
			string originalXml = textReader.ReadToEnd();
			string validXml = XmlHelper.ReEncodeAttributeValues(originalXml);
			var stringReader = new StringReader(validXml);

			var testCaseContainer = new TestCaseContainer();
			XDocument doc = XDocument.Load(stringReader);

			// Check for <testcases>
			XElement rootElement = doc.Elements().FirstOrDefault(i => i.Name.LocalName == "testcases");
			if (rootElement == null)
				throw new TestCaseException("<testcases> node is missing from the config file.");

			// Repeats
			int repeatValue = 0;
			string repeatAttribute = XmlHelper.GetOptionalAttribute(rootElement, "repeat");
			int.TryParse(repeatAttribute, out repeatValue);
			testCaseContainer.Repeat = repeatValue;

			// <testvar>
			testCaseContainer.Variables = GetTestVars(rootElement);

			// <case> - add each  one and re-order them by their id="" attribute.
			var testCases = new List<TestCase>();
			foreach (XElement element in rootElement.Elements().Where(x => x.Name.LocalName == "case"))
			{
				TestCase testCase = GetTestCase(element);
				testCases.Add(testCase);
			}
			testCaseContainer.TestCases = testCases.OrderBy(x => x.Id);

			return testCaseContainer;
		}

		private Dictionary<string, string> GetTestVars(XElement rootElement)
		{
			// Example: <testvar varname="LOGIN_URL">x</testvar>
			var variables = new Dictionary<string, string>();

			foreach (XElement element in rootElement.Elements().Where(x => x.Name.LocalName == "testvar"))
			{
				XAttribute varnameAttribute = element.Attributes("varname").FirstOrDefault();
				if (varnameAttribute != null)
				{
					if (!variables.ContainsKey(varnameAttribute.Value))
					{
						variables.Add(varnameAttribute.Value, element.Value);
					}
				}
			}

			return variables;
		}

		private TestCase GetTestCase(XElement element)
		{
			var testCase = new TestCase();

			// Required Properties
			testCase.Id = AttributeAsInt(element, "id");
			testCase.Url = XmlHelper.GetOptionalAttribute(element, "url");
			if (string.IsNullOrEmpty(testCase.Url))
				throw new TestCaseException("The url parameter is missing for test case {0}", testCase.Id);

			// Optionals
			testCase.Method = XmlHelper.GetOptionalAttribute(element, "method", "get");
			testCase.PostBody = XmlHelper.GetOptionalAttribute(element, "postbody");
			testCase.ErrorMessage = XmlHelper.GetOptionalAttribute(element, "errormessage");
			testCase.PostType = XmlHelper.GetOptionalAttribute(element, "posttype", "application/x-www-form-urlencoded");
			testCase.VerifyResponseCode = GetVerifyResponseCode(element);
			testCase.LogRequest = YesToBool(element, "logrequest");
			testCase.LogResponse = YesToBool(element, "logresponse");
			testCase.Sleep = AttributeAsInt(element, "sleep");
			testCase.AddHeader = ParseAddHeader(element);

			// Numbered attributes
			testCase.Descriptions = GetNumberedAttributes(element, "description");
			testCase.ParseResponses = GetNumberedAttributes(element, "parseresponse");
			testCase.VerifyPositives = GetNumberedAttributes(element, "verifypositive");
			testCase.VerifyNegatives = GetNumberedAttributes(element, "verifynegative");

			return testCase;
		}

		private List<KeyValuePair<string, string>> ParseAddHeader(XElement element)
		{
			var addHeaders = new List<KeyValuePair<string, string>>();

			// Headers are in the format mykey: 12345|bar: foo
			string attributeValue = XmlHelper.GetOptionalAttribute(element, "addheader");
			if (!string.IsNullOrEmpty(attributeValue) && attributeValue.Contains(":"))
			{
				if (attributeValue.Contains(":"))
				{
					if (attributeValue.Contains("|"))
					{
						string[] headers = attributeValue.Split('|');
						foreach (string item in headers)
						{
							addHeaders.Add(GetHeaderPair(item));
						}
					}
					else
					{
						addHeaders.Add(GetHeaderPair(attributeValue));
					}
				}
			}

			return addHeaders;
		}

		private KeyValuePair<string, string> GetHeaderPair(string header)
		{
			string[] parts = header.Split(':');
			if (parts.Length == 2)
			{
				return new KeyValuePair<string, string>(parts[0].Trim(), parts[1].Trim());
			}

			return new KeyValuePair<string, string>();
		}

		private bool YesToBool(XElement element, string attributeName)
		{
			string attributeValue = XmlHelper.GetOptionalAttribute(element, attributeName, "yes");
			return (attributeValue == "yes");
		}

		private HttpStatusCode GetVerifyResponseCode(XElement element)
		{
			int attributeValue = AttributeAsInt(element, "verifyresponsecode", 200);

			HttpStatusCode statusCode = HttpStatusCode.OK;
			if (Enum.IsDefined(typeof(HttpStatusCode), attributeValue))
			{
				Enum.TryParse(attributeValue.ToString(), out statusCode);
			}

			return statusCode;
		}

		private int AttributeAsInt(XElement element, string attributeName, int defaultValue = 0)
		{
			string idValue = XmlHelper.GetOptionalAttribute(element, attributeName);
			int result = 0;
			if (!int.TryParse(idValue, out result))
				result = defaultValue;

			return result;
		}

		internal List<string> GetNumberedAttributes(XElement element, string attributeName)
		{
			if (string.IsNullOrEmpty(attributeName) || !element.HasAttributes)
				return new List<string>();

			var items = new List<KeyValuePair<int, string>>();

			//
			// Take the attributes (e.g. description1="", description3="", description2="") and put them into an ordered list
			//
			IEnumerable<XAttribute> attributes = element.Attributes().Where(x => x.Name.LocalName.ToLower().StartsWith(attributeName));
			foreach (XAttribute attribute in attributes)
			{
				int index = 0;

				string currentAttributeName = attribute.Name.LocalName.ToLower();
				currentAttributeName = currentAttributeName.Replace(attributeName, "");
				if (!string.IsNullOrEmpty(attributeName))
				{
					int.TryParse(currentAttributeName, out index);
				}

				items.Add(new KeyValuePair<int, string>(index, attribute.Value));
			}

			return items.OrderBy(x => x.Key)
						.Select(x => x.Value)
						.ToList();
		}
	}
}
