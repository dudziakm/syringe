using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;

namespace Syringe.Core.Http.Logging
{
	public class HttpLogWriter : IHttpLogWriter, IDisposable
	{
		private static readonly string REQUEST_LINE_FORMAT = "{0} {1} HTTP/1.1";
		private static readonly string HEADER_FORMAT = "{0}: {1}";
		private static readonly string RESPONSE_LINE_FORMAT = "HTTP/1.1 {0} {1}";

		protected internal readonly TextWriter Writer;
		protected internal string Seperator { get; set; }

		public HttpLogWriter(TextWriter writer)
		{
			//_streamWriter = new StreamWriter(new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write));
			Writer = writer;
			Seperator = "************************* LOG SEPARATOR *************************";
		}

		public virtual void AppendSeperator()
		{
			Writer.WriteLine(Seperator);
		}

		public virtual void AppendRequest(string method, string url, IEnumerable<KeyValuePair<string, string>> headers)
		{
			if (string.IsNullOrEmpty(method))
				return;

			if (string.IsNullOrEmpty(url))
				return;

			if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
				return;

			Uri uri = new Uri(url);
			Writer.WriteLine(REQUEST_LINE_FORMAT, method.ToUpper(), url);
			Writer.WriteLine(HEADER_FORMAT, "Host", uri.Host);


			if (headers != null)
			{
				foreach (var keyValuePair in headers)
				{
					Writer.WriteLine(HEADER_FORMAT, keyValuePair.Key, keyValuePair.Value);
				}
			}

			Writer.WriteLine();
		}

		public virtual void AppendResponse(HttpStatusCode status, IEnumerable<KeyValuePair<string, string>> headers, string bodyResponse)
		{
			int statusCode = (int)status;
			Writer.WriteLine(RESPONSE_LINE_FORMAT, statusCode, HttpWorkerRequest.GetStatusDescription(statusCode));

			if (headers != null)
			{
				foreach (var keyValuePair in headers)
				{
					Writer.WriteLine(HEADER_FORMAT, keyValuePair.Key, keyValuePair.Value);
				}
			}

			Writer.WriteLine();

			if (!string.IsNullOrEmpty(bodyResponse))
				Writer.WriteLine(bodyResponse);
		}

		public void Dispose()
		{
			Writer.Dispose();
		}
	}
}
