using System;
using System.IO;
using Newtonsoft.Json;
using Syringe.Core.Exceptions;

namespace Syringe.Core.Configuration
{
	public class JsonConfigurationStore : IConfigurationStore
	{
		private IConfiguration _configuration;
		private readonly string _configPath;

		public JsonConfigurationStore()
		{
			_configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configuration.json");
		}

		public IConfiguration Load()
		{
			if (_configuration == null)
			{
				string json = File.ReadAllText(_configPath);
				JsonConfiguration configuration = JsonConvert.DeserializeObject<JsonConfiguration>(json);

				_configuration = configuration;
			}

			return _configuration;
		}
	}
}