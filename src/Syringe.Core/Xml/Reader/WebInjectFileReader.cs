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
    public class WebInjectFileReader : ITestFileReader
    {
        public TestFile Read(TextReader textReader)
        {
            var testFile = new TestFile();
            XDocument doc = XDocument.Load(textReader);

            // Check for <tests>
            XElement rootElement = doc.Elements().FirstOrDefault(i => i.Name.LocalName == "testcases");
            if (rootElement == null)
                throw new TestException("<testcases> node is missing from the config file.");


            // <variables>
            testFile.Variables = GetVariables(rootElement);

            var tests = new List<Test>();
            List<XElement> elements = doc.Elements().Where(x => x.Name.LocalName == "case").ToList();
            for (int i = 0; i < elements.Count; i++)
            {
                XElement element = elements[i];
                Test test = GetTest(element, i);
                test.AvailableVariables = testFile.Variables.Select(x => new Variable { Name = x.Name, Value = x.Value }).ToList();
                test.AvailableVariables.AddRange(testFile.Tests.SelectMany(x => x.CapturedVariables).Select(x => new Variable { Name = x.Name, Value = x.Regex }));
                tests.Add(test);
            }
            testFile.Tests = tests;

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

        private Test GetTest(XElement element, int position)
        {
            var test = new Test();

            // Required Properties
            test.Position = position;
            test.Url = XmlHelper.GetOptionalAttribute(element, "url");
            if (string.IsNullOrEmpty(test.Url))
                throw new TestException("The url parameter is missing for test case {0}", test.Position);

            // Optionals
            test.Method = XmlHelper.GetOptionalAttribute(element, "method", "get");
            test.PostBody = XmlHelper.GetOptionalElementValue(element, "postbody").Trim();
            test.ErrorMessage = XmlHelper.GetOptionalAttribute(element, "errormessage");
            test.PostType = XmlHelper.GetOptionalAttribute(element, "posttype", "application/x-www-form-urlencoded");
            test.VerifyResponseCode = GetVerifyResponseCode(element);
            test.Headers = GetHeaders(element);

            // Descriptions
            test.ShortDescription = XmlHelper.GetOptionalAttribute(element, "description1");
            test.LongDescription = XmlHelper.GetOptionalAttribute(element, "description2");

            test.CapturedVariables = GetCapturedVariables(element);

            test.Assertions = GetAssertions(element);

            return test;
        }

        private List<HeaderItem> GetHeaders(XElement testElement)
        {
            var headers = new List<HeaderItem>();

            var variableElement = testElement.Elements().Where(x => x.Name.LocalName == "headers");

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

        private HttpStatusCode GetVerifyResponseCode(XElement testElement)
        {
            int attributeValue = XmlHelper.AttributeAsInt(testElement, "verifyresponsecode", 200);

            HttpStatusCode statusCode = HttpStatusCode.OK;
            if (Enum.IsDefined(typeof(HttpStatusCode), attributeValue))
            {
                Enum.TryParse(attributeValue.ToString(), out statusCode);
            }

            return statusCode;
        }

        private List<CapturedVariable> GetCapturedVariables(XElement testElement)
        {
            var items = new List<CapturedVariable>();
            var parentElement = testElement.Elements().Where(x => x.Name.LocalName == "capturedvariables");

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

        private List<Assertion> GetAssertions(XElement testElement)
        {
            var items = new List<Assertion>();

            foreach (XAttribute element in testElement.Attributes().Where(x => x.Name.LocalName.Contains("verify")))
            {
                AssertionType assertionType = testElement.Name.LocalName.Contains("positive") ? AssertionType.Positive : AssertionType.Negative;
                items.Add(new Assertion(element.Name.LocalName, element.Value, assertionType));
            }

            return items;
        }
    }
}
