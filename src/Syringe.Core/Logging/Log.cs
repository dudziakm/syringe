using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Syringe.Core.Logging
{
	public class Log
	{
		private static readonly Logger _logger;
		private static readonly string SIMPLE_LAYOUT = "${message}";
		private static readonly string FILE_LAYOUT = "[${date:universalTime=true:format=dd-MM-yyyy HH\\:mm\\:ss}] [${level}] ${message}";

		private static readonly string LOGGER_NAME = "Syringe";

		static Log()
		{
			_logger = LogManager.GetLogger(LOGGER_NAME);
		}

		public static void All()
		{
			var configuration = new LoggingConfiguration();

			AddConsoleLogging(configuration);
			AddChainSawLogging(configuration);
			AddTextfileLogging(configuration);

			LogManager.Configuration = configuration;
		}

		public static void Enable()
		{
			LogManager.EnableLogging();
		}

		public static void Disable()
		{
			LogManager.DisableLogging();
		}

		/// <summary>
		/// Adds ConsoleTraceListener logging to the logging listeners.
		/// </summary>
		private static void AddConsoleLogging(LoggingConfiguration configuration)
		{
			ConsoleTarget target = new ConsoleTarget();
			target.Layout = SIMPLE_LAYOUT;
			configuration.AddTarget("Console", target);

			LoggingRule rule = new LoggingRule("*", LogLevel.Debug, target);
			configuration.LoggingRules.Add(rule);
		}

		/// <summary>
		/// Adds network logging.
		/// </summary>
		public static void AddChainSawLogging(LoggingConfiguration configuration)
		{
			ChainsawTarget target = new ChainsawTarget();
			target.AppInfo = "Syringe";
			target.Address = "udp://127.0.0.1:7071";
			configuration.AddTarget("Chainsaw", target);

			LoggingRule rule = new LoggingRule("*", LogLevel.Debug, target);
			configuration.LoggingRules.Add(rule);
		}

		/// <summary>
		/// Adds rolling file logging.
		/// </summary>
		public static void AddTextfileLogging(LoggingConfiguration configuration)
		{
			FileTarget target = new FileTarget();
			target.FileName = @"${basedir}\syringe.log";
			target.Layout = FILE_LAYOUT;
			target.KeepFileOpen = false;
			target.ArchiveNumbering = ArchiveNumberingMode.Rolling;
			configuration.AddTarget("File", target);

			LoggingRule rule = new LoggingRule("*", LogLevel.Debug, target);
			configuration.LoggingRules.Add(rule);
		}

		/// <summary>
		/// Creates an information log message.
		/// </summary>
		public static void Debug(string message, params object[] args)
		{
			WriteLine(Level.Debug, null, message, args);
		}

		/// <summary>
		/// Creates an information log message.
		/// </summary>
		public static void Information(string message, params object[] args)
		{
			WriteLine(Level.Information, null, message, args);
		}

		/// <summary>
		/// Creates an information log message, also logging the provided exception.
		/// </summary>
		public static void Information(Exception ex, string message, params object[] args)
		{
			WriteLine(Level.Information, ex, message, args);
		}

		/// <summary>
		/// Creates a warning log message.
		/// </summary>
		public static void Warn(string message, params object[] args)
		{
			WriteLine(Level.Warning, null, message, args);
		}

		/// <summary>
		/// Creates a information log message, also logging the provided exception.
		/// </summary>
		public static void Warn(Exception ex, string message, params object[] args)
		{
			WriteLine(Level.Warning, ex, message, args);
		}

		/// <summary>
		/// Creates an error (e.g. application crash) log message.
		/// </summary>
		public static void Error(string message, params object[] args)
		{
			WriteLine(Level.Error, null, message, args);
		}

		/// <summary>
		/// Creates an error (e.g. application crash) log message, also logging the provided exception.
		/// </summary>
		public static void Error(Exception ex, string message, params object[] args)
		{
			WriteLine(Level.Error, ex, message, args);
		}

		/// <summary>
		/// Writes a log message for the <see cref="Level"/>, and if the provided Exception is not null,
		/// appends this exception to the message.
		/// </summary>
		public static void WriteLine(Level errorType, Exception ex, string message, params object[] args)
		{
			if (ex != null)
				message += "\n" + ex;

			switch (errorType)
			{
				case Level.Warning:
					_logger.Warn(message, args);
					break;

				case Level.Error:
					_logger.Error(message, args);
					break;

				case Level.Information:
					_logger.Info(message, args);
					break;

				case Level.Debug:
				default:
					_logger.Debug(message, args);
					break;
			}
		}
	}
}
