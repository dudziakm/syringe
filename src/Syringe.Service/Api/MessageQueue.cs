using System.Collections.Concurrent;

namespace Syringe.Service.Api
{
	public class MessageQueue
	{
		public ConcurrentQueue<string> Queue { get; private set; }

		public MessageQueue()
		{
			Queue = new ConcurrentQueue<string>();
		}

		public void Add(string message)
		{
			Queue.Enqueue(message);
		}

		public string GetNext()
		{
			string result = "";
			if (Queue.TryDequeue(out result))
			{
				return result;
			}
			else
			{
				return "An error occurred";
			}
		}
	}
}