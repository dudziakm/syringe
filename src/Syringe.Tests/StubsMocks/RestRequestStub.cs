using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using RestSharp;
using RestSharp.Serializers;

namespace Syringe.Tests.StubsMocks
{
	public class RestRequestStub : IRestRequest
	{
		public bool AlwaysMultipartFormData { get; set; }
		public ISerializer JsonSerializer { get; set; }
		public ISerializer XmlSerializer { get; set; }
		public Action<Stream> ResponseWriter { get; set; }
		public List<Parameter> Parameters { get; }
		public List<FileParameter> Files { get; }
		public Method Method { get; set; }
		public string Resource { get; set; }
		public DataFormat RequestFormat { get; set; }
		public string RootElement { get; set; }
		public string DateFormat { get; set; }
		public string XmlNamespace { get; set; }
		public ICredentials Credentials { get; set; }
		public int Timeout { get; set; }
		public int ReadWriteTimeout { get; set; }
		public int Attempts { get; }
		public bool UseDefaultCredentials { get; set; }
		public Action<IRestResponse> OnBeforeDeserialization { get; set; }

		public IRestRequest AddFile(string name, string path, string contentType = null)
		{
			return this;
		}

		public IRestRequest AddFile(string name, byte[] bytes, string fileName, string contentType = null)
		{
			return this;
		}

		public IRestRequest AddFile(string name, Action<Stream> writer, string fileName, string contentType = null)
		{
			return this;
		}

		public IRestRequest AddBody(object obj, string xmlNamespace)
		{
			return this;
		}

		public IRestRequest AddBody(object obj)
		{
			return this;
		}

		public IRestRequest AddJsonBody(object obj)
		{
			return this;
		}

		public IRestRequest AddXmlBody(object obj)
		{
			return this;
		}

		public IRestRequest AddXmlBody(object obj, string xmlNamespace)
		{
			return this;
		}

		public IRestRequest AddObject(object obj, params string[] includedProperties)
		{
			return this;
		}

		public IRestRequest AddObject(object obj)
		{
			return this;
		}

		public IRestRequest AddParameter(Parameter p)
		{
			return this;
		}

		public IRestRequest AddParameter(string name, object value)
		{
			return this;
		}

		public IRestRequest AddParameter(string name, object value, ParameterType type)
		{
			return this;
		}

		public IRestRequest AddHeader(string name, string value)
		{
			return this;
		}

		public IRestRequest AddCookie(string name, string value)
		{
			return this;
		}

		public IRestRequest AddUrlSegment(string name, string value)
		{
			return this;
		}

		public IRestRequest AddQueryParameter(string name, string value)
		{
			return this;
		}

		public void IncreaseNumAttempts()
		{
		}
	}
}
