using System.Web.Http;
using Owin;
using Swashbuckle.Application;

namespace Syringe.Service
{
    public class WebPipeline
    {
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