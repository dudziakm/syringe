using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Syringe.Web
{
	/*
	Example usage:

	<system.webServer>
		<handlers accessPolicy = "Read, Execute, Script" >
			< add name="lesshandler" path="*.less" verb="GET" type="Syringe.Web.LessHttpHandler, Syringe.Web" resourceType="File" preCondition="" />
		</handlers>
	</system.webServer>
	*/
	public class LessHttpHandler : IHttpHandler
	{
		public bool IsReusable
		{
			get { return true; }
		}


		public void ProcessRequest(HttpContext context)
		{
			string cssPath = ReplaceLessFile(context.Request.Url.LocalPath);
			string physicalPath = Path.GetFileName(cssPath);

			context.Response.ContentType = "text/css";
			context.Response.TransmitFile(physicalPath);
		}

		private string ReplaceLessFile(string lessFilename)
		{
			string result = lessFilename.Replace(".less", ".css");

#if !DEBUG
			result = result.Replace(".css", ".min.css");
#endif

			return result;
		}
	}
}