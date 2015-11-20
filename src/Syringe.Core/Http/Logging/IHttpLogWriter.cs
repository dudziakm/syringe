namespace Syringe.Core.Http.Logging
{
	public interface IHttpLogWriter
	{
		void AppendSeperator();
		void AppendRequest(RequestDetails requestDetails);
		void AppendResponse(ResponseDetails responseDetails);
	}
}