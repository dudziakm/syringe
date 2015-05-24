using System.Runtime.Caching;
using System.Web.Http;
using Syringe.Service.Models;

namespace Syringe.Service.Api
{
    public class MessageController : ApiController
    {
		[Route("api/messages/send")]
		public void Send([FromUri] MessageDetails message)
		{
			//MessageQueue.Messages.Add(message);
		}

        [Route("api/run")]
        [HttpGet]
        public void Run()
        {
            //MessageQueue.Messages.Add(message);
        }
    }
}