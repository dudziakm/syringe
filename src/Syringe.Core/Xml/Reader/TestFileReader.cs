using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Syringe.Core.Exceptions;
using Syringe.Core.Tests;

namespace Syringe.Core.Xml.Reader
{
    public class TestFileReader : ITestFileReader
    {
        public TestFile Read(TextReader textReader)
        {
            var testFile = new TestFile();
            XDocument doc = XDocument.Load(textReader);

            // Check for <testcases>
            XElement rootElement = doc.Elements().FirstOrDefault(i => i.Name.LocalName == "tests");
            if (rootElement == null)
                throw new TestCaseException("<tests> node is missing from the config file.");

            // Repeats
            int repeatValue = 0;
            string repeatAttribute = XmlHelper.GetOptionalAttribute(rootElement, "repeat");
            int.TryParse(repeatAttribute, out repeatValue);
            testFile.Repeat = repeatValue;

            // <variables>
            testFile.Variables = GetVariables(rootElement);

            // <tests> - add each one and re-order them by their id="" attribute.
            var testCases = new List<Test>();
            var elements = rootElement.Elements().Where(x => x.Name.LocalName == "test").ToList();
            for (int i = 0; i < elements.Count; i++)
            {
                XElement element = elements[i];
                Test test = GetTest(element);
                testCases.Add(test);
            }
            testFile.Tests = testCases;

            return testFile;
        }

        private List<Variable> GetVariables(XElement rootElement)
        {
            // <variables>
            //      <variable name="login" environment="DevTeam1"></variable>
            // </variables>

            var variables = new List<Variable>();
            var variableElement = rootElement.Elements().Where(x => x.Name.LocalName == "variables");

            foreach (XElement element in variableElement.Elements().Where(x => x.Name.LocalName == "variable"))
            {
                XAttribute nameAttribute = element.Attributes("name").FirstOrDefault();
                if (nameAttribute != null)
                {
					XAttribute environmentAttribute = element.Attributes("environment").FirstOrDefault();
	                string environment = "";
	                if (environmentAttribute != null)
		                environment = environmentAttribute.Value;

					if (!variables.Any(x => x.Name.Equals(nameAttribute.Value, StringComparison.InvariantCultureIgnoreCase)))
					{
						variables.Add(new Variable(nameAttribute.Value, element.Value, environment));
					}
				}
            }

            return variables;
        }

        private Test GetTest(XElement element)
        {
            var test = new Test();

            // Required Properties
            test.Id = XmlHelper.AttributeAsGuid(element, "id");
            test.Url = XmlHelper.GetOptionalAttribute(element, "url");
            if (string.IsNullOrEmpty(test.Url))
                throw new TestCaseException("The url parameter is missing for test case {0}", test.Id);

            // Optionals
            test.Method = XmlHelper.GetOptionalAttribute(element, "method", "get");
            test.PostBody = XmlHelper.GetOptionalElementValue(element, "postbody").Trim();
            test.ErrorMessage = XmlHelper.GetOptionalAttribute(element, "errormessage");
            test.PostType = XmlHelper.GetOptionalAttribute(element, "posttype", "application/x-www-form-urlencoded");
            test.VerifyResponseCode = GetVerifyResponseCode(element);
            test.Headers = GetHeaders(element);

            // Descriptions
            test.ShortDescription = XmlHelper.GetOptionalAttribute(element, "shortdescription");
            test.LongDescription = XmlHelper.GetOptionalAttribute(element, "longdescription");

            test.CapturedVariables = GetCapturedVariables(element);

            List<Assertion> verifications = GetAssertions(element);
            test.VerifyPositives = verifications.Where(x => x.AssertionType == AssertionType.Positive).ToList();
            test.VerifyNegatives = verifications.Where(x => x.AssertionType == AssertionType.Negative).ToList();

            return test;
        }

        private List<HeaderItem> GetHeaders(XElement caseElement)
        {
            var headers = new List<HeaderItem>();

            var variableElement = caseElement.Elements().Where(x => x.Name.LocalName == "headers");

            foreach (XElement element in variableElement.Elements().Where(x => x.Name.LocalName == "header"))
            {
                XAttribute nameAttribute = element.Attributes("name").FirstOrDefault();
                if (nameAttribute != null)
                {
                    headers.Add(new HeaderItem(nameAttribute.Value, element.Value));
                }
            }

            return headers;
        }

        private bool GetBoolValue(XElement element, string attributeName)
        {
            string attributeValue = XmlHelper.GetOptionalAttribute(element, attributeName, "true");
            return (attributeValue.Equals("true", StringComparison.OrdinalIgnoreCase));
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

        private List<CapturedVariable> GetCapturedVariables(XElement caseElement)
        {
            var items = new List<CapturedVariable>();
            var parentElement = caseElement.Elements().Where(x => x.Name.LocalName == "capturedvariables");

            foreach (XElement element in parentElement.Elements().Where(x => x.Name.LocalName == "variable"))
            {
                XAttribute descriptionAttribute = element.Attributes("description").FirstOrDefault();
                string description = "";

                if (descriptionAttribute != null)
                    description = descriptionAttribute.Value;

                items.Add(new CapturedVariable(description, element.Value));
            }

            return items;
        }

        private List<Assertion> GetAssertions(XElement caseElement)
        {
            var items = new List<Assertion>();
            var parentElement = caseElement.Elements().Where(x => x.Name.LocalName == "assertions");

            foreach (XElement element in parentElement.Elements().Where(x => x.Name.LocalName == "assertion"))
            {
                XAttribute descriptionAttribute = element.Attributes("description").FirstOrDefault();
                string description = "";

                if (descriptionAttribute != null)
                    description = descriptionAttribute.Value;

                XAttribute verifyTypeAttribute = element.Attributes("type").FirstOrDefault();
                AssertionType assertionType = AssertionType.Positive;

                if (verifyTypeAttribute != null)
                {
                    Enum.TryParse(verifyTypeAttribute.Value, true, out assertionType);
                }

                items.Add(new Assertion(description, element.Value, assertionType));
            }

            return items;
        }
    }
}
