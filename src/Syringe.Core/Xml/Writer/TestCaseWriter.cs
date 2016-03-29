using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Syringe.Core.TestCases;

namespace Syringe.Core.Xml.Writer
{
    public class TestCaseWriter : ITestCaseWriter
    {
        public string Write(CaseCollection caseCollection)
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
                    XElement testCasesElement = new XElement("testcases");
                    testCasesElement.Add(new XAttribute("repeat", caseCollection.Repeat.ToString()));

                    if (caseCollection.Variables.Count > 0)
                    {
                        XElement variableElements = new XElement("variables");

                        foreach (var variable in caseCollection.Variables)
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

                    foreach (Case testCase in caseCollection.TestCases)
                    {
                        XElement headersElement = GetHeadersElement(testCase);
                        XElement postbodyElement = GetPostBodyElement(testCase);
                        XElement parseResponsesElement = GetParseResponsesElement(testCase);
                        XElement verificationElement = GetVerificationElement(testCase);

                        XElement caseElement = GetCaseElement(testCase);
                        caseElement.Add(headersElement);
                        caseElement.Add(postbodyElement);
                        caseElement.Add(parseResponsesElement);
                        caseElement.Add(verificationElement);

                        testCasesElement.Add(caseElement);
                    }

                    XDocument doc = new XDocument(testCasesElement);
                    doc.WriteTo(xmlWriter);
                }

                return stringBuilder.ToString();
            }
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

            return element;
        }

        private XElement GetHeadersElement(Case testCase)
        {
            XElement headerElement = new XElement("headers");

            foreach (HeaderItem keyValuePair in testCase.Headers)
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

        private XElement GetPostBodyElement(Case testCase)
        {
            XElement postBodyElement = new XElement("postbody");

            if (!string.IsNullOrEmpty(testCase.PostBody))
                postBodyElement.Add(new XCData(testCase.PostBody));

            return postBodyElement;
        }

        private XElement GetParseResponsesElement(Case testCase)
        {
            XElement parseresponsesElement = new XElement("parseresponses");

            foreach (ParseResponseItem item in testCase.ParseResponses)
            {
                if (!string.IsNullOrEmpty(item.Regex))
                {
                    XElement element = new XElement("parseresponse");
                    element.Add(new XAttribute("description", item.Description));
                    element.Value = item.Regex;

                    parseresponsesElement.Add(element);
                }
            }

            return parseresponsesElement;
        }

        private XElement GetVerificationElement(Case testCase)
        {
            XElement headerElement = new XElement("verifications");

            foreach (VerificationItem verifyItem in testCase.VerifyPositives.Union(testCase.VerifyNegatives))
            {
                if (!string.IsNullOrEmpty(verifyItem.Description))
                {
                    XElement element = new XElement("verify");
                    element.Add(new XAttribute("description", verifyItem.Description));
                    element.Add(new XAttribute("type", verifyItem.VerifyType.ToString().ToLower()));

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