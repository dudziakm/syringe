namespace Syringe.Core.Repositories.MongoDB
{
	public class Configuration
	{
		public string ConnectionString { get; set; }
		public string DatabaseName { get; set; }

		public Configuration()
		{
			ConnectionString = "mongodb://localhost:27017";
			DatabaseName = "Syringe";
        }
	}
}