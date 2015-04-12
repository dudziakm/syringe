using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using Syringe.Core.Exceptions;

namespace Syringe.Core.Xml
{
	internal class XmlHelper
	{
		private static Regex _attributeRegex;

		private static Regex AttributeRegex
		{
			get
			{
				if (_attributeRegex == null)
				{
					string attributes = "verifypositive[0-9]{0,}|" +
					                    "verifynegative[0-9]{0,}|" +
					                    "parseresponse[0-9]{0,1}|" +
										"verifynextpositive|" +
										"verifynextnegative|" +
					                    "url|" +
										"addheader|" +
					                    "postbody";
					_attributeRegex = new Regex("(" +attributes+ @")+=\""(.*?)\""", RegexOptions.Compiled | RegexOptions.IgnoreCase);
				}

				return _attributeRegex;
			}
		}

		public static string GetRequiredElementValue(XElement rootElement, string name)
		{
			var element = rootElement.Elements().FirstOrDefault(x => x.Name.LocalName == name);
			if (element == null)
				throw new XmlException("The element <{0}> is missing", name);

			return element.Value;
		}

		public static string GetOptionalElementValue(XElement rootElement, string name)
		{
			var element = rootElement.Elements().FirstOrDefault(x => x.Name.LocalName == name);
			if (element != null)
				return element.Value;

			return "";
		}

		public static string GetRequiredAttribute(XElement rootElement, string attributeName)
		{
			XAttribute attribute = rootElement.Attribute(attributeName);
			if (attribute == null)
				throw new XmlException("The {0} attribute is missing", attributeName);

			return attribute.Value;
		}

		public static string GetOptionalAttribute(XElement rootElement, string attributeName)
		{
			XAttribute attribute = rootElement.Attribute(attributeName);
			if (attribute != null)
				return attribute.Value;

			return "";
		}

		public static string ReEncodeAttributeValues(string xmlElement)
		{
			// See 3.7: http://www.webinject.org/manual.html#tcvalidxml
			// - webinject allows & and "\<" "\>" in attribute values (so it's more readable), which is invalid XML.
			string result = AttributeRegex.Replace(xmlElement, match =>
			{
				string attributeName = match.Groups[1].Value;
				string attributeValue = match.Groups[2].Value;
				attributeValue = attributeValue.Replace(@"\<", "&lt;")
											   .Replace(@"\>", "&gt;");

				attributeValue = HttpUtility.HtmlEncode(HttpUtility.HtmlDecode(attributeValue));

				return attributeName+ "=\"" + attributeValue + "\"";
			});


			return result;
		}

		public static List<string> GetOrderedAttributes(XElement element, string attributeName)
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