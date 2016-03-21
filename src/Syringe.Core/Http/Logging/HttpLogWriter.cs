using System;
using System.IO;
using System.Text;
using System.Web;

namespace Syringe.Core.Http.Logging
{
	public class HttpLogWriter
	{
		private static readonly string REQUEST_LINE_FORMAT = "{0} {1} HTTP/1.1";
		private static readonly string HEADER_FORMAT = "{0}: {1}";
		private static readonly string RESPONSE_LINE_FORMAT = "HTTP/1.1 {0} {1}";
		private readonly StringWriter _writer;

		public StringBuilder StringBuilder { get; set; }

		public HttpLogWriter()
		{
			StringBuilder = new StringBuilder();
			_writer = new StringWriter(StringBuilder);
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
			_writer.WriteLine(REQUEST_LINE_FORMAT, requestDetails.Method.ToUpper(), requestDetails.Url);
			_writer.WriteLine(HEADER_FORMAT, "Host", uri.Host);

			if (requestDetails.Headers != null)
			{
				foreach (var keyValuePair in requestDetails.Headers)
				{
					_writer.WriteLine(HEADER_FORMAT, keyValuePair.Key, keyValuePair.Value);
				}
			}

			if (!string.IsNullOrEmpty(requestDetails.Body))
				_writer.WriteLine(requestDetails.Body);

			_writer.WriteLine();
		}

		public virtual void AppendResponse(ResponseDetails responseDetails)
		{
			int statusCode = (int)responseDetails.Status;
			_writer.WriteLine(RESPONSE_LINE_FORMAT, statusCode, HttpWorkerRequest.GetStatusDescription(statusCode));

			if (responseDetails.Headers != null)
			{
				foreach (var keyValuePair in responseDetails.Headers)
				{
					_writer.WriteLine(HEADER_FORMAT, keyValuePair.Key, keyValuePair.Value);
				}
			}

			_writer.WriteLine();

			if (!string.IsNullOrEmpty(responseDetails.BodyResponse))
				_writer.WriteLine(responseDetails.BodyResponse);
		}
	}
}
