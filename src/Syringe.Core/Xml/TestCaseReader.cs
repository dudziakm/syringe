using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			XElement rootElement = doc.Root;
			if (rootElement == null)
				throw new TestCaseException("<testcases> node is missing from the config file.");

			// Repeats
			int repeatValue = 0;
			string repeatAttribute = XmlHelper.GetOptionalAttribute(rootElement, "repeat");
			int.TryParse(repeatAttribute, out repeatValue);
			testCaseContainer.Repeat = repeatValue;

			// Testvars: all <testvar varname="LOGIN_URL">x</testvar>
			testCaseContainer.Variables = GetTestVars(rootElement);

			// Each <case> - order them by id="" attribute.
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
			var testcase = new TestCase();

			// Id
			string idValue = XmlHelper.GetOptionalAttribute(element, "id");
			int id = 0;
			int.TryParse(idValue, out id);
			testcase.Id = id;

			// Descriptions
			testcase.Descriptions = GetOrderedAttributes(element, "description");
			

			return testcase;
		}

		internal List<string> GetOrderedAttributes(XElement element, string attributeName)
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
