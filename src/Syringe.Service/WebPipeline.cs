using System.Web.Http;
using Owin;

namespace Syringe.Service
{
    public class WebPipeline
    {
        public void Configuration(IAppBuilder application)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            application.UseWebApi(config);
        }
    }
}