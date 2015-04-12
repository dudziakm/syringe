using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Syringe.Core.Exceptions;

namespace Syringe.Core.Xml
{
	internal class XmlHelper
	{
		private static readonly Regex _attributeRegex = new Regex("=([\"']){1}(.*?)\\1", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public static string GetRequiredElementValue(XElement rootElement, string name)
		{
			var element = rootElement.Elements().FirstOrDefault(x => x.Name.LocalName == name);
			if (element == null)
				throw new Syringe.Core.Exceptions.XmlException("The element <{0}> is missing", name);

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
				throw new Syringe.Core.Exceptions.XmlException("The {0} attribute is missing", attributeName);

			return attribute.Value;
		}

		public static string GetOptionalAttribute(XElement rootElement, string attributeName, string defaultValue = "")
		{
			XAttribute attribute = rootElement.Attribute(attributeName);
			if (attribute != null)
				return attribute.Value;

			return defaultValue;
		}

		public static string ReEncodeAttributeValues(string xml)
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