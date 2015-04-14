using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;

namespace Syringe.Core.Http.Logging
{
	public class HttpLogWriter : IHttpLogWriter
	{
		private static readonly string REQUEST_LINE_FORMAT = "{0} {1} HTTP/1.1";
		private static readonly string HEADER_FORMAT = "{0}: {1}";
		private static readonly string RESPONSE_LINE_FORMAT = "HTTP/1.1 {0} {1}";

		protected internal readonly ITextWriterFactory TextWriterFactory;
		protected internal string Seperator { get; set; }

		public HttpLogWriter(ITextWriterFactory textWriterFactory)
		{
			TextWriterFactory = textWriterFactory;
			Seperator = "************************* LOG SEPARATOR *************************";
		}

		public virtual void WriteSeperator()
		{
			using (TextWriter textWriter = TextWriterFactory.GetWriter())
			{
				textWriter.WriteLine(Seperator);
			}
		}

		public virtual void WriteRequest(string method, string url, IEnumerable<KeyValuePair<string, string>> headers)
		{
			if (string.IsNullOrEmpty(method))
				return;

			if (string.IsNullOrEmpty(url))
				return;

			if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
				return;

			using (TextWriter textWriter = TextWriterFactory.GetWriter())
			{
				Uri uri = new Uri(url);
				textWriter.WriteLine(REQUEST_LINE_FORMAT, method.ToUpper(), url);
				textWriter.WriteLine(HEADER_FORMAT, "Host", uri.Host);

				if (headers != null)
				{
					foreach (var keyValuePair in headers)
					{
						textWriter.WriteLine(HEADER_FORMAT, keyValuePair.Key, keyValuePair.Value);
					}
				}

				textWriter.WriteLine();
			}
		}

		public virtual void WriteResponse(HttpStatusCode status, IEnumerable<KeyValuePair<string, string>> headers, string bodyResponse)
		{
			using (TextWriter textWriter = TextWriterFactory.GetWriter())
			{
				int statusCode = (int) status;
				textWriter.WriteLine(RESPONSE_LINE_FORMAT, statusCode, HttpWorkerRequest.GetStatusDescription(statusCode));

				if (headers != null)
				{
					foreach (var keyValuePair in headers)
					{
						textWriter.WriteLine(HEADER_FORMAT, keyValuePair.Key, keyValuePair.Value);
					}
				}

				textWriter.WriteLine();

				if (!string.IsNullOrEmpty(bodyResponse))
					textWriter.WriteLine(bodyResponse);
			}
		}
	}
}
