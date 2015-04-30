using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Syringe.Core.Exceptions;
using Syringe.Core.Logging;

namespace Syringe.Core.Xml
{
	public class LegacyTestCaseReader : ITestCaseReader, IDisposable
	{
		private static readonly Regex _attributeRegex = new Regex("=([\"']){1}(.*?)\\1", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private readonly TextReader _textReader;

	    public LegacyTestCaseReader(TextReader textReader)
	    {
		    _textReader = textReader;
	    }

		public CaseCollection Read()
		{
			// Clean up any invalid XML
			string originalXml = _textReader.ReadToEnd();
			string validXml = ReEncodeAttributeValues(originalXml);
			var stringReader = new StringReader(validXml);

			var testCollection = new CaseCollection();
			XDocument doc = XDocument.Load(stringReader);

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
			testCase.PostBody = XmlHelper.GetOptionalAttribute(element, "postbody");
			testCase.ErrorMessage = XmlHelper.GetOptionalAttribute(element, "errormessage");
			testCase.PostType = XmlHelper.GetOptionalAttribute(element, "posttype", "application/x-www-form-urlencoded");
			testCase.VerifyResponseCode = GetVerifyResponseCode(element);
			testCase.LogRequest = YesToBool(element, "logrequest");
			testCase.LogResponse = YesToBool(element, "logresponse");
			testCase.Sleep = XmlHelper.AttributeAsInt(element, "sleep");
			testCase.Headers = ParseAddHeader(element);

			// Descriptions - support either description,description1 or description1/description2
			testCase.ShortDescription = XmlHelper.GetOptionalAttribute(element, "description1");
			testCase.LongDescription = XmlHelper.GetOptionalAttribute(element, "description2");

			if (string.IsNullOrEmpty(testCase.ShortDescription))
			{
				testCase.ShortDescription = XmlHelper.GetOptionalAttribute(element, "description");

				if (string.IsNullOrEmpty(testCase.LongDescription))
				{
					testCase.LongDescription = XmlHelper.GetOptionalAttribute(element, "description1");
				}
			}

			// Numbered attributes
			List<ParsedResponseItem> parsedResponses = GetParsedResponseItems(element, "parseresponse");
			testCase.ParseResponses = ConvertParseResponsesToRegexes(parsedResponses);
			testCase.VerifyPositives = GetVerificationItems(element, "verifypositive", VerifyType.Positive);
			testCase.VerifyNegatives = GetVerificationItems(element, "verifynegative", VerifyType.Negative);

			return testCase;
		}

		private Dictionary<string, string> GetTestVars(XElement rootElement)
		{
            // <variables>
            //  <variable name="login"></variable>
            // </variables>

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
            // Parses the header, which is in the format "mykey: 12345|bar: foo|emptyvalue:|Cookie: referer=harrispilton.com"
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
			int attributeValue = XmlHelper.AttributeAsInt(element, "verifyresponsecode", 200);

			HttpStatusCode statusCode = HttpStatusCode.OK;
			if (Enum.IsDefined(typeof(HttpStatusCode), attributeValue))
			{
				Enum.TryParse(attributeValue.ToString(), out statusCode);
			}

			return statusCode;
		}

		internal List<ParsedResponseItem> GetParsedResponseItems(XElement element, string attributeName)
		{
			if (string.IsNullOrEmpty(attributeName) || !element.HasAttributes)
				return new List<ParsedResponseItem>();

			var items = new List<KeyValuePair<int, string>>();

			//
			// Take the attributes (e.g. parsedresponse1="", parsedresponse3="", parsedresponse2="") and put them into an ordered list
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
						.Select(x => new ParsedResponseItem(attributeName + x.Key, x.Value))
						.ToList();
		}

		internal List<VerificationItem> GetVerificationItems(XElement element, string attributeName, VerifyType verifyType)
		{
			if (string.IsNullOrEmpty(attributeName) || !element.HasAttributes)
				return new List<VerificationItem>();

			var items = new List<KeyValuePair<int, string>>();

			//
			// Take the attributes (e.g. verifypositive1="", verifypositive3="", verifypositive2="") and put them into an ordered list
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
						.Select(x => new VerificationItem(attributeName + x.Key, x.Value, verifyType))
						.ToList();
		}

		internal List<ParsedResponseItem> ConvertParseResponsesToRegexes(List<ParsedResponseItem> parseResponses)
		{
			var responseVariables = new List<ParsedResponseItem>();

			// From the manual:
			// 
			// Parse a string from the HTTP response for use in subsequent requests. This is mostly used for passing Session ID's, 
			// but can be applied to any case where you need to pass a dynamically generated value. It takes the arguments in the format 
			// "leftboundary|rightboundary", and an optional third argument "leftboundary|rightboundary|escape" when you want to 
			// force escaping of all non-alphanumeric characters

			// Example:
			//      <input type="hidden" name="__VIEWSTATE" value="dDwtMTA4NzczMzUxMjs7Ps1HmLfiYGewI+2JaAxhcpiCtj52" />
			//
			//      parseresponse='__VIEWSTATE" value="|"|escape'
			//
			// This is then used by:
			//      postbody="value=123&__VIEWSTATE={PARSEDRESULT}"
			//
			// This as a regex:
			//		parseresponse='__VIEWSTATE" value="(.*?)'

			for (int i = 0; i < parseResponses.Count; i++)
			{
				ParsedResponseItem item = parseResponses[i];
				string parsedResponse = item.Regex;
				if (parsedResponse.Contains("|"))
				{
					string[] parts = parsedResponse.Split('|');

					// Regex escape the 2 chunks
					string firstChunk = Regex.Escape(parts[0]);
					string secondChunk = Regex.Escape(parts[1]);

					// Reconstruct as a regex
					string regexText = string.Format("{0}(.*?){1}", firstChunk, secondChunk);
					try
					{
						var testRegex = new Regex(regexText);
						responseVariables.Add(new ParsedResponseItem("parsedresponse" +i, regexText));
					}
					catch (ArgumentException e)
					{
						Log.Information("ParsedResponse conversion to a regex failed: {0}", regexText);
						responseVariables.Add(item);
					}
				}
				else
				{
					// What is the behaviour when there is no pipe?
					responseVariables.Add(item);
				}
			}

			return responseVariables;
		}

		internal static string ReEncodeAttributeValues(string xml)
		{
			// See 3.7: http://www.webinject.org/manual.html#tcvalidxml
			// - webinject allows & and "\<" "\>" in attribute values (so it's more readable), which is invalid XML.

			// Do a cheap replace
			xml = xml.Replace(@"=>", "=&gt;");
			xml = xml.Replace(@"\<", "&lt;");
			xml = xml.Replace(@"\>", "&gt;");
			xml = xml.Replace(@"\""", "&quot;");
			xml = xml.Replace("&nbsp;", XmlConvert.EncodeName("&nbsp;")); // TODO: replace back to &nbsp;

			// Go through all attributes, and re-encode them.
			foreach (Match match in _attributeRegex.Matches(xml))
			{
				string attvalue = match.Groups[2].Value;

				if (!string.IsNullOrEmpty(attvalue))
				{
					string valid = HttpUtility.HtmlEncode(HttpUtility.HtmlDecode(attvalue));
					xml = xml.Replace(attvalue, valid);
				}
			}

			return xml;
		}

		internal static List<string> GetOrderedAttributes(XElement element, string attributeName)
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

		public void Dispose()
	    {
		    _textReader.Dispose();
	    }
	}
}
