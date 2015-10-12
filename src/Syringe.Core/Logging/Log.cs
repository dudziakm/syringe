using System;
using NLog;
using NLog.Config;

namespace Syringe.Core.Logging
{
	public class Log
	{
		private static readonly Logger _errorLogger;

		static Log()
		{
			_errorLogger = LogManager.GetLogger("Syringe-Errors");
		}

		public static void UseConsole()
		{
			var configuration = new LoggingConfiguration();
			var configurationBuilder = new NLogConfigurationBuilder(configuration);

			configurationBuilder.AddConsoleLogging("Syringe-Errors");

			LogManager.Configuration = configuration;
		}

		public static void UseAllTargets()
		{
			var configuration = new LoggingConfiguration();
			var configurationBuilder = new NLogConfigurationBuilder(configuration);

			configurationBuilder.AddConsoleLogging("Syringe-Errors");
			configurationBuilder.AddChainSawLogging("Syringe-Errors");
			configurationBuilder.AddTextfileLogging("Syringe-Errors", "syringe.log");

			LogManager.Configuration = configuration;
		}

		/// <summary>
		/// Creates an information log message.
		/// </summary>
		public static void Information(string message, params object[] args)
		{
			WriteLine(Level.Debug, null, message, args);
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
					_errorLogger.Warn(message, args);
					break;

				case Level.Error:
					_errorLogger.Error(message, args);
					break;

				case Level.Information:
					_errorLogger.Info(message, args);
					break;

				case Level.Debug:
				default:
					_errorLogger.Debug(message, args);
					break;
			}
		}
	}
}
