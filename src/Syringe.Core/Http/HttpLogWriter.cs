using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syringe.Core.Http
{
	public class HttpLogWriter
	{
		private static readonly string REQUEST_LINE_FORMAT = "{0} {1} HTTP/1.1";
		private static readonly string REQUEST_HEADER_FORMAT = "{0}: {1}";

		private readonly ITextWriterFactory _textWriterFactory;

		public HttpLogWriter(ITextWriterFactory textWriterFactory)
		{
			_textWriterFactory = textWriterFactory;
		}

		public void WriteHeader()
		{

		}

		public void WriteRequest(string method, string url, IEnumerable<KeyValuePair<string, string>> headers)
		{
			if (string.IsNullOrEmpty(method))
				return;

			if (string.IsNullOrEmpty(url))
				return;

			if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
				return;

			using (TextWriter textWriter = _textWriterFactory.GetWriter())
			{
				Uri uri = new Uri(url);
				textWriter.WriteLine(REQUEST_LINE_FORMAT, method.ToUpper(), url);
				textWriter.WriteLine(REQUEST_HEADER_FORMAT, "Host", uri.Host);

				if (headers != null)
				{
					foreach (var keyValuePair in headers)
					{
						textWriter.WriteLine(REQUEST_HEADER_FORMAT, keyValuePair.Key, keyValuePair.Value);
					}
				}

				textWriter.WriteLine();
			}
		}

		public void WriterResponse()
		{
//HTTP/1.1 200 OK
//Server: Apache
//X-Content-Type-Options: nosniff
//X-Analytics: page_id=19001;ns=0
//Content-language: en
//X-UA-Compatible: IE=Edge
//Vary: Accept-Encoding,Cookie
//X-Powered-By: HHVM/3.3.1
//Last-Modified: Sun, 12 Apr 2015 14:11:21 GMT
//Content-Type: text/html; charset=UTF-8
//X-Varnish: 4107227022 4107212816, 661483373 661449298, 949501881 907135891
//Via: 1.1 varnish, 1.1 varnish, 1.1 varnish
//Content-Length: 425942
//Accept-Ranges: bytes
//Date: Sun, 12 Apr 2015 19:18:21 GMT
//Age: 18382
//Connection: keep-alive
//X-Cache: cp1065 hit (1), cp3041 hit (2), cp3030 frontend hit (46)
//Cache-Control: private, s-maxage=0, max-age=0, must-revalidate

//<!DOCTYPE html>
//<html lang="en" dir="ltr" class="client-nojs">

		}
	}
}
