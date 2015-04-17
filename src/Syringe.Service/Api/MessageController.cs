using System.Web.Http;
using Syringe.Service.Models;

namespace Syringe.Service.Api
{
    public class MessageController : ApiController
    {
        //[Route("api/messages/send")]
        //public void Send([FromUri] MessageDetails message)
        //{
        //    //MessageQueue.Messages.Add(message);
        //}
        [Route("api/test")]
        [HttpGet]
        public void Test()
        {
            //MessageQueue.Messages.Add(message);
        }
    }
}