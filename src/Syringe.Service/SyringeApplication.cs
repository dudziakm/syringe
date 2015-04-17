using System;
using Microsoft.Owin.Hosting;

namespace Syringe.Service
{
    public class SyringeApplication
    {
        protected IDisposable WebApplication;
        public void Start()
        {
            WebApplication = WebApp.Start<WebPipeline>("http://localhost:1232");
        }

        public void Stop()
        {
            WebApplication.Dispose();
        }
    }
}