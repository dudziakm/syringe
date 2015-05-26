using System;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;
using Swashbuckle.Application;
using Syringe.Service.Parallel;

namespace Syringe.Service
{
    public class SyringeApplication
    {
        protected IDisposable WebApplication;

        public void Start()
        {
			WebApplication = WebApp.Start<SyringeApplication>("http://localhost:1232");
        }

        public void Stop()
        {
			ParallelTestSessionQueue.Default.StopAll();
            WebApplication.Dispose();
        }

		public void Configuration(IAppBuilder application)
		{
			var config = new HttpConfiguration();
			config.EnableSwagger(c => c.SingleApiVersion("v1", "Syringe runner REST API"))
				  .EnableSwaggerUi();

			config.MapHttpAttributeRoutes();
			application.UseWebApi(config);
		}
    }
}