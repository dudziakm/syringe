namespace Syringe.Core.Repositories.RavenDB
{
	public class RavenDBConfiguration
	{
		public string Url { get; set; }
		public string DefaultDatabase { get; set; }

		public RavenDBConfiguration()
		{
			Url = "http://localhost:32345";
			DefaultDatabase = "syringe";
        }
	}
}