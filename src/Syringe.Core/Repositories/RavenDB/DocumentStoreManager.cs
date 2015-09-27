using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client;
using Raven.Client.Document;

namespace Syringe.Core.Repositories.RavenDB
{
	public static class DocumentStoreManager
	{
		private static readonly Lazy<IDocumentStore> _documentStore = new Lazy<IDocumentStore>(() =>
		{
			var docStore = new DocumentStore
			{
				ConnectionStringName = "RavenDB"
			};
			docStore.Initialize();

			return docStore;
		});

		public static IDocumentStore DocumentStore
		{
			get { return _documentStore.Value; }
		}
	}
}
