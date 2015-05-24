using System;
using System.Web.Mvc;
using NServiceBus;
using Syringe.Core.ServiceBus;

namespace Syringe.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Run()
        {
			// http://docs.particular.net/samples/hosting/windows-service/
			BusConfiguration busConfiguration = new BusConfiguration();
			busConfiguration.EndpointName("Syringe.Web.Controllers");
			busConfiguration.UseSerialization<JsonSerializer>();

			var startableBus = Bus.Create(busConfiguration);
			var bus = startableBus.Start();
			bus.Send(new Address("foo", "localhost"), new StartTestCaseCommand() { Id = Guid.NewGuid(), TestCaseFilename = "test.xml"});

            return Content("started ");
        }
    }
}