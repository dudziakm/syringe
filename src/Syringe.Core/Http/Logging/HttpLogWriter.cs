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
			Writer = writer;

			// Webinject's default seperator
			Seperator = "************************* LOG SEPARATOR *************************";
		}

		public virtual void AppendSeperator()
		{
			Writer.WriteLine(Seperator);
		}

		public virtual void AppendRequest(RequestDetails requestDetails)
		{
			if (string.IsNullOrEmpty(requestDetails.Method))
				return;

			if (string.IsNullOrEmpty(requestDetails.Url))
				return;

			if (!Uri.IsWellFormedUriString(requestDetails.Url, UriKind.Absolute))
				return;

			Uri uri = new Uri(requestDetails.Url);
			Writer.WriteLine(REQUEST_LINE_FORMAT, requestDetails.Method.ToUpper(), requestDetails.Url);
			Writer.WriteLine(HEADER_FORMAT, "Host", uri.Host);

			if (requestDetails.Headers != null)
			{
				foreach (var keyValuePair in requestDetails.Headers)
				{
					Writer.WriteLine(HEADER_FORMAT, keyValuePair.Key, keyValuePair.Value);
				}
			}

			if (!string.IsNullOrEmpty(requestDetails.Body))
				Writer.WriteLine(requestDetails.Body);

			Writer.WriteLine();
		}

		public virtual void AppendResponse(ResponseDetails responseDetails)
		{
			int statusCode = (int)responseDetails.Status;
			Writer.WriteLine(RESPONSE_LINE_FORMAT, statusCode, HttpWorkerRequest.GetStatusDescription(statusCode));

			if (responseDetails.Headers != null)
			{
				foreach (var keyValuePair in responseDetails.Headers)
				{
					Writer.WriteLine(HEADER_FORMAT, keyValuePair.Key, keyValuePair.Value);
				}
			}

			Writer.WriteLine();

			if (!string.IsNullOrEmpty(responseDetails.BodyResponse))
				Writer.WriteLine(responseDetails.BodyResponse);
		}

		public void Dispose()
		{
			Writer.Dispose();
		}
	}
}
