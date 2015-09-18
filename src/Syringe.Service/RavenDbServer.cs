using Raven.Client;
using Raven.Client.Embedded;
using Raven.Database.Server;

namespace Syringe.Service
{
	/// <summary>
	/// http://localhost:32345/studio/index.html#databases/documents?&database=syringe
	/// </summary>
	public class RavenDbServer
	{
		public static IDocumentStore DocumentStore = new EmbeddableDocumentStore
		{
			DefaultDatabase = "syringe",
			DataDirectory = @"C:\RavenDb\",
			UseEmbeddedHttpServer = true,
			Configuration = { Port = 32345 }
		};

		public static void Start()
		{
			NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(32345);
			DocumentStore.Initialize();
		}

		public static void Stop()
		{
			DocumentStore.Dispose();
		}
	}
}