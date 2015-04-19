using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Syringe.Core.Exceptions;

namespace Syringe.Core.Xml
{
    public class TestCaseReader : ITestCaseReader
    {
		public TestCaseCollection Read(TextReader textReader)
		{
			var testCollection = new TestCaseCollection();
            XDocument doc = XDocument.Load(textReader);

			// Check for <testcases>
			XElement rootElement = doc.Elements().FirstOrDefault(i => i.Name.LocalName == "testcases");
			if (rootElement == null)
				throw new TestCaseException("<testcases> node is missing from the config file.");

			// Repeats
			int repeatValue = 0;
			string repeatAttribute = XmlHelper.GetOptionalAttribute(rootElement, "repeat");
			int.TryParse(repeatAttribute, out repeatValue);
			testCollection.Repeat = repeatValue;

			// <testvar>
			testCollection.Variables = GetTestVars(rootElement);

			// <case> - add each  one and re-order them by their id="" attribute.
			var testCases = new List<TestCase>();
			foreach (XElement element in rootElement.Elements().Where(x => x.Name.LocalName == "case"))
			{
				TestCase testCase = GetTestCase(element);
				testCases.Add(testCase);
			}
			testCollection.TestCases = testCases.OrderBy(x => x.Id);

			return testCollection;
		}

		private Dictionary<string, string> GetTestVars(XElement rootElement)
		{
            // <variables>
            //      <variable name="login"></variable>
            // </variables>

			var variables = new Dictionary<string, string>();
		    var variableElement = rootElement.Elements().Where(x => x.Name.LocalName == "variables");

            foreach (XElement element in variableElement.Elements().Where(x => x.Name.LocalName == "variable"))
			{
				XAttribute varnameAttribute = element.Attributes("name").FirstOrDefault();
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
			testCase.Id = XmlHelper.AttributeAsInt(element, "id");
			testCase.Url = XmlHelper.GetOptionalAttribute(element, "url");
			if (string.IsNullOrEmpty(testCase.Url))
				throw new TestCaseException("The url parameter is missing for test case {0}", testCase.Id);

			// Optionals
			testCase.Method = XmlHelper.GetOptionalAttribute(element, "method", "get");
			testCase.PostBody = XmlHelper.GetOptionalElementValue(element, "postbody").Trim();
			testCase.ErrorMessage = XmlHelper.GetOptionalAttribute(element, "errormessage");
			testCase.PostType = XmlHelper.GetOptionalAttribute(element, "posttype", "application/x-www-form-urlencoded");
			testCase.VerifyResponseCode = GetVerifyResponseCode(element);
			testCase.LogRequest = YesToBool(element, "logrequest");
			testCase.LogResponse = YesToBool(element, "logresponse");
			testCase.Sleep = XmlHelper.AttributeAsInt(element, "sleep");
			testCase.Headers = GetHeader(element);

			// Numbered attributes
			testCase.ShortDescription = XmlHelper.GetOptionalAttribute(element, "shortdescription");
			testCase.LongDescription = XmlHelper.GetOptionalAttribute(element, "longdescription");

            testCase.ParseResponses = GetElementCollection(element, "parseresponse", "parseresponses", "parseresponse");
            testCase.VerifyPositives = GetElementCollection(element, "verifypositive", "verifypositives", "verify");
            testCase.VerifyNegatives = GetElementCollection(element, "verifynegative", "verifynegatives", "verify");

			return testCase;
		}

		private List<KeyValuePair<string, string>> GetHeader(XElement caseElement)
		{
			var headers = new List<KeyValuePair<string, string>>();

            var variableElement = caseElement.Elements().Where(x => x.Name.LocalName == "headers");

            foreach (XElement element in variableElement.Elements().Where(x => x.Name.LocalName == "header"))
            {
                XAttribute nameAttribute = element.Attributes("name").FirstOrDefault();
                if (nameAttribute != null)
                {
                    headers.Add(new KeyValuePair<string, string>(nameAttribute.Value, element.Value));
                }
            }

			return headers;
		}

		private bool YesToBool(XElement element, string attributeName)
		{
			string attributeValue = XmlHelper.GetOptionalAttribute(element, attributeName, "yes");
			return (attributeValue == "yes");
		}

		private HttpStatusCode GetVerifyResponseCode(XElement caseElement)
		{
			int attributeValue = XmlHelper.AttributeAsInt(caseElement, "verifyresponsecode", 200);

			HttpStatusCode statusCode = HttpStatusCode.OK;
			if (Enum.IsDefined(typeof(HttpStatusCode), attributeValue))
			{
				Enum.TryParse(attributeValue.ToString(), out statusCode);
			}

			return statusCode;
		}

        private List<RegexItem> GetElementCollection(XElement caseElement, string name, string parentElementName, string childElementName)
		{
            if (string.IsNullOrEmpty(childElementName))
				return new List<RegexItem>();

			var variables = new List<RegexItem>();
            var variableElement = caseElement.Elements().Where(x => x.Name.LocalName == parentElementName);

            foreach (XElement element in variableElement.Elements().Where(x => x.Name.LocalName == childElementName))
            {
				XAttribute descriptionAttribute = element.Attributes("description").FirstOrDefault();
	            string description = "";

				if (descriptionAttribute != null)
					description = descriptionAttribute.Value;

				variables.Add(new RegexItem(description, element.Value));
            }

		    return variables;
		}
	}
}
