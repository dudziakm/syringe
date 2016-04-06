using NUnit.Framework;
using Syringe.Client;

namespace Syringe.Tests.Integration.ClientAndService
{
	[TestFixture]
	public class ConfigurationClientTests
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			ServiceConfig.StartSelfHostedOwin();
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			ServiceConfig.OwinServer.Dispose();
		}

		[Test]
		public void GetConfiguration_should_get_full_config_object()
		{
			// given
			var client = new ConfigurationClient(ServiceConfig.BaseUrl);

			// when
			var config = client.GetConfiguration();

			// then
			Assert.That(config, Is.Not.Null);
		}
	}
}
