using NLog;
using NLog.Config;
using NLog.Targets;

namespace Syringe.Core.Logging
{
	internal class NLogConfigurationBuilder
	{
		private readonly LoggingConfiguration _configuration;
		private static readonly string SIMPLE_LAYOUT = "${message}";
		private static readonly string FILE_LAYOUT = "[${date:universalTime=true:format=dd-MM-yyyy HH\\:mm\\:ss}] [${level}] ${message}";

		public NLogConfigurationBuilder(LoggingConfiguration configuration)
		{
			_configuration = configuration;
		}

		/// <summary>
		/// Adds ConsoleTraceListener logging to the logging listeners.
		/// </summary>
		public void AddConsoleLogging(string loggerName)
		{
			ConsoleTarget target = new ConsoleTarget();
			target.Layout = SIMPLE_LAYOUT;
			_configuration.AddTarget("Console", target);

			LoggingRule rule = new LoggingRule(loggerName, LogLevel.Debug, target);
			_configuration.LoggingRules.Add(rule);
		}

		/// <summary>
		/// Adds network logging.
		/// </summary>
		public void AddChainSawLogging(string loggerName)
		{
			ChainsawTarget target = new ChainsawTarget();
			target.AppInfo = "Syringe";
			target.Address = "udp://127.0.0.1:7071";
			_configuration.AddTarget("Chainsaw", target);

			LoggingRule rule = new LoggingRule(loggerName, LogLevel.Debug, target);
			_configuration.LoggingRules.Add(rule);
		}

		/// <summary>
		/// Adds rolling file logging.
		/// </summary>
		public void AddTextfileLogging(string loggerName, string filename)
		{
			FileTarget target = new FileTarget();
			target.FileName = @"${basedir}\" +filename;
			target.Layout = FILE_LAYOUT;
			target.KeepFileOpen = false;
			target.ArchiveNumbering = ArchiveNumberingMode.Rolling;
			_configuration.AddTarget("File", target);

			LoggingRule rule = new LoggingRule(loggerName, LogLevel.Debug, target);
			_configuration.LoggingRules.Add(rule);
		}
	}
}