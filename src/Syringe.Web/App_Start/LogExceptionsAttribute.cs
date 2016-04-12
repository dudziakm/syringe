using System.Web.Mvc;
using Syringe.Core.Logging;

namespace Syringe.Web
{
	public class LogExceptionsAttribute : HandleErrorAttribute
	{
		public override void OnException(ExceptionContext filterContext)
		{
			Log.Error(filterContext.Exception, "An MVC error occurred");
		}
	}
}