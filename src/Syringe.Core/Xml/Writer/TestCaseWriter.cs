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
					XElement caseElement = GetCaseElement(testCase);
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
			element.Add(new XAttribute("shortdescription", testCase.ShortDescription));
			element.Add(new XAttribute("longdescription", testCase.LongDescription));
			element.Add(new XAttribute("url", testCase.Url));
			element.Add(new XAttribute("method", testCase.Method));
			element.Add(new XAttribute("posttype", testCase.PostType));
			element.Add(new XAttribute("verifyresponsecode", (int)testCase.VerifyResponseCode));
			element.Add(new XAttribute("errormessage", testCase.ErrorMessage));
			element.Add(new XAttribute("logrequest", testCase.LogRequest));
			element.Add(new XAttribute("logresponse", testCase.LogResponse));
			element.Add(new XAttribute("sleep", testCase.Sleep));

			return element;
		}
	}
}