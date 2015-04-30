using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Syringe.Core.Exceptions;

namespace Syringe.Core.Xml
{
    public class TestCaseReader : ITestCaseReader, IDisposable
    {
	    private readonly TextReader _textReader;

	    public TestCaseReader(TextReader textReader)
	    {
		    _textReader = textReader;
	    }

	    public CaseCollection Read()
		{
			var testCollection = new CaseCollection();
			XDocument doc = XDocument.Load(_textReader);

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
			var testCases = new List<Case>();
			foreach (XElement element in rootElement.Elements().Where(x => x.Name.LocalName == "case"))
			{
				Case testCase = GetTestCase(element);
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

		private Case GetTestCase(XElement element)
		{
			var testCase = new Case();

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

            testCase.ParseResponses = GetParsedResponseCollection(element);

			List<VerificationItem> verifications = GetVerificationCollection(element);
			testCase.VerifyPositives = verifications.Where(x => x.VerifyType == VerifyType.Positive).ToList();
			testCase.VerifyNegatives = verifications.Where(x => x.VerifyType == VerifyType.Negative).ToList();

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

        private List<ParsedResponseItem> GetParsedResponseCollection(XElement caseElement)
		{
			var items = new List<ParsedResponseItem>();
			var parentElement = caseElement.Elements().Where(x => x.Name.LocalName == "parseresponses");

			foreach (XElement element in parentElement.Elements().Where(x => x.Name.LocalName == "parseresponse"))
            {
				XAttribute descriptionAttribute = element.Attributes("description").FirstOrDefault();
	            string description = "";

				if (descriptionAttribute != null)
					description = descriptionAttribute.Value;

				items.Add(new ParsedResponseItem(description, element.Value));
            }

		    return items;
		}

		private List<VerificationItem> GetVerificationCollection(XElement caseElement)
		{
			var items = new List<VerificationItem>();
			var parentElement = caseElement.Elements().Where(x => x.Name.LocalName == "verifications");

			foreach (XElement element in parentElement.Elements().Where(x => x.Name.LocalName == "verify"))
			{
				XAttribute descriptionAttribute = element.Attributes("description").FirstOrDefault();
				string description = "";

				if (descriptionAttribute != null)
					description = descriptionAttribute.Value;

				XAttribute verifyTypeAttribute = element.Attributes("type").FirstOrDefault();
				VerifyType verifyType = VerifyType.Positive;

				if (verifyTypeAttribute != null)
				{
					Enum.TryParse(verifyTypeAttribute.Value, true, out verifyType);
				}

				items.Add(new VerificationItem(description, element.Value, verifyType));
			}

			return items;
		}

	    public void Dispose()
	    {
		    _textReader.Dispose();
	    }
    }
}
