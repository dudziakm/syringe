using Topshelf;

namespace Syringe.Service
{
    /// <summary>
    /// Influence from https://msdn.microsoft.com/en-us/magazine/dn745865.aspx
    /// </summary>
    class Program
    {
        static int Main(string[] args)
        {
            TopshelfExitCode exitCode = HostFactory.Run(host =>
            {
                host.Service<SyringeApplication>(service =>
                {
                    service.ConstructUsing(() => new SyringeApplication());
                    service.WhenStarted(x => x.Start());
                    service.WhenStopped(x => x.Stop());
                });

                host.SetDescription("This super amazing host and runner of tests");
                host.SetDisplayName("Syringe.Service");
                host.RunAsNetworkService();
            });
            return (int)exitCode;
        }
    }
}
