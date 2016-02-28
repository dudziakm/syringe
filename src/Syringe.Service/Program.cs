using Syringe.Service.DependencyResolution;
using Topshelf;

namespace Syringe.Service
{
	/// <summary>
	/// Influence from https://msdn.microsoft.com/en-us/magazine/dn745865.aspx
	/// </summary>
	internal class Program
	{
		public static int Main(string[] args)
		{
			// .\Syringe.Service.exe [verb] [-option:value] [-switch]
			// run Runs the service from the command line (default)
			// help or –help Displays help
			// install Installs the service

		    var container = IoC.Initialize();

			TopshelfExitCode exitCode = HostFactory.Run(host =>
			{
				host.Service<SyringeApplication>(service =>
				{
					service.ConstructUsing(() => container.GetInstance<SyringeApplication>());
					service.WhenStarted(x => x.Start());
					service.WhenStopped(x => x.Stop());
				});

				host.SetDescription("This super amazing host and runner of tests");
				host.SetDisplayName("Syringe.Service");
				host.RunAsNetworkService();
			});
			return (int) exitCode;
		}
	}
}
