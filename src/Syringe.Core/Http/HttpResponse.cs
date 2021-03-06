﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Syringe.Core.Http
{
	public class HttpResponse
	{
		public HttpStatusCode StatusCode { get; set; }
		public string Content { get; set; }
		public List<KeyValuePair<string, string>> Headers { get; set; } // *not a dictionary* - it can have duplicate keys
		public TimeSpan ResponseTime { get; set; }

		public HttpResponse()
		{
			Headers = new List<KeyValuePair<string, string>>();
		}

		public override string ToString()
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("HTTP/1.1 {0} OK", (int)StatusCode));

			foreach (KeyValuePair<string, string> keyValuePair in Headers)
			{
				stringBuilder.AppendLine(string.Format("{0}: {1}", keyValuePair.Key, keyValuePair.Value));
			}

			stringBuilder.AppendLine();
			stringBuilder.Append(Content);

			return stringBuilder.ToString();
		}
	}
}