using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Syringe.Core.Tests;

namespace Syringe.Core.Xml.Writer
{
    public class TestFileWriter : ITestFileWriter
    {
        public string Write(TestFile testFile)
        {
            var stringBuilder = new StringBuilder();
            using (var stringWriter = new Utf8StringWriter(stringBuilder))
            {
                var settings = new XmlWriterSettings
                {
                    IndentChars = "\t",
                    Indent = true
                };

                using (XmlWriter xmlWriter = XmlTextWriter.Create(stringWriter, settings))
                {
                    XElement testCasesElement = new XElement("tests");
                    testCasesElement.Add(new XAttribute("repeat", testFile.Repeat.ToString()));

                    if (testFile.Variables.Count > 0)
                    {
                        XElement variableElements = new XElement("variables");

                        foreach (var variable in testFile.Variables)
                        {
                            XElement variableElement = new XElement("variable");
                            variableElement.Add(new XAttribute("name", variable.Name));
                            variableElement.Value = variable.Value;

							variableElement.Add(new XAttribute("environment", variable.Environment.Name));
							variableElement.Value = variable.Value;

							variableElements.Add(variableElement);
                        }

                        testCasesElement.Add(variableElements);
                    }

                    foreach (Test testCase in testFile.Tests)
                    {
                        XElement headersElement = GetHeadersElement(testCase);
                        XElement postbodyElement = GetPostBodyElement(testCase);
                        XElement parseResponsesElement = GetParseResponsesElement(testCase);
                        XElement assertionElement = GetAssertionsElement(testCase);

                        XElement caseElement = GetCaseElement(testCase);
                        caseElement.Add(headersElement);
                        caseElement.Add(postbodyElement);
                        caseElement.Add(parseResponsesElement);
                        caseElement.Add(assertionElement);

                        testCasesElement.Add(caseElement);
                    }

                    XDocument doc = new XDocument(testCasesElement);
                    doc.WriteTo(xmlWriter);
                }

                return stringBuilder.ToString();
            }
        }

        private XElement GetCaseElement(Test testTest)
        {
            XElement element = new XElement("case");

            element.Add(new XAttribute("shortdescription", testTest.ShortDescription ?? ""));
            element.Add(new XAttribute("longdescription", testTest.LongDescription ?? ""));
            element.Add(new XAttribute("url", testTest.Url ?? ""));
            element.Add(new XAttribute("method", testTest.Method ?? ""));
            element.Add(new XAttribute("posttype", testTest.PostType ?? ""));
            element.Add(new XAttribute("verifyresponsecode", (int)testTest.VerifyResponseCode));
            element.Add(new XAttribute("errormessage", testTest.ErrorMessage ?? ""));

            return element;
        }

        private XElement GetHeadersElement(Test testTest)
        {
            XElement headerElement = new XElement("headers");

            foreach (HeaderItem keyValuePair in testTest.Headers)
            {
                if (!string.IsNullOrEmpty(keyValuePair.Key))
                {
                    XElement element = new XElement("header");
                    element.Add(new XAttribute("name", keyValuePair.Key));

                    AddCDataToElementValue(keyValuePair.Value, element);

                    headerElement.Add(element);
                }
            }

            return headerElement;
        }

        private XElement GetPostBodyElement(Test testTest)
        {
            XElement postBodyElement = new XElement("postbody");

            if (!string.IsNullOrEmpty(testTest.PostBody))
                postBodyElement.Add(new XCData(testTest.PostBody));

            return postBodyElement;
        }

        private XElement GetParseResponsesElement(Test testTest)
        {
            XElement parseresponsesElement = new XElement("capturedvariables");

            foreach (CapturedVariable item in testTest.CapturedVariables)
            {
                if (!string.IsNullOrEmpty(item.Regex))
                {
                    XElement element = new XElement("variable");
                    element.Add(new XAttribute("description", item.Name));
                    element.Value = item.Regex;

                    parseresponsesElement.Add(element);
                }
            }

            return parseresponsesElement;
        }

        private XElement GetAssertionsElement(Test testTest)
        {
            XElement headerElement = new XElement("assertions");

            foreach (Assertion verifyItem in testTest.VerifyPositives.Union(testTest.VerifyNegatives))
            {
                if (!string.IsNullOrEmpty(verifyItem.Description))
                {
                    XElement element = new XElement("assertion");
                    element.Add(new XAttribute("description", verifyItem.Description));
                    element.Add(new XAttribute("type", verifyItem.AssertionType.ToString().ToLower()));

                    AddCDataToElementValue(verifyItem.Regex, element);

                    headerElement.Add(element);
                }
            }

            return headerElement;
        }

        private static void AddCDataToElementValue(string value, XElement element)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Contains("&") ||
                    value.Contains("<") || value.Contains(">"))
                {
                    element.Add(new XCData(value));
                }
                else
                {
                    element.Value = value;
                }
            }
        }
    }
}