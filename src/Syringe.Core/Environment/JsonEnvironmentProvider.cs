using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Syringe.Core.Environment
{
	public class JsonEnvironmentProvider : IEnvironmentProvider
	{
		private List<Environment> _environments;
		private readonly string _configPath;

		public JsonEnvironmentProvider()
		{
			_configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "environments.json");
		}

		internal JsonEnvironmentProvider(string configPath)
		{
			_configPath = configPath;
		}

		public IEnumerable<Environment> GetAll()
		{
			if (_environments == null)
			{
				if (File.Exists(_configPath))
				{
					string json = File.ReadAllText(_configPath);
					List<Environment> environments = JsonConvert.DeserializeObject<List<Environment>>(json);

					_environments = environments.OrderBy(x => x.Order).ToList();
				}
				else
				{
					_environments = new List<Environment>();
				}
			}

			return _environments;
		}
	}
}