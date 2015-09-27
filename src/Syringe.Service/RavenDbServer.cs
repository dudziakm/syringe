using System;
using System.IO;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Database.Server;

namespace Syringe.Service
{
	/// <summary>
	/// To manage RavenDB, go to http://localhost:8087/
	/// </summary>
	public class RavenDbServer
	{
		public static IDocumentStore DocumentStore = new EmbeddableDocumentStore
		{
			DefaultDatabase = "syringe",
			DataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RavenDB"),
			UseEmbeddedHttpServer = true,
			Configuration = { Port = 8087 }
		};

		public static void Start()
		{
			NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8087);
			DocumentStore.Initialize();
		}

		public static void Stop()
		{
			DocumentStore.Dispose();
		}
	}
}