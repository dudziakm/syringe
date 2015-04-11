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
				throw new ConfigurationException("The element <{0}> is missing", name);

			return element.Value;
		}

		public static string GetOptionalElementValue(XElement rootElement, string name)
		{
			var element = rootElement.Elements().FirstOrDefault(x => x.Name.LocalName == name);
			if (element != null)
				return element.Value;

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
	}
}