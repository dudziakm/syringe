using System;
using System.IO;
using System.Reflection;
using Syringe.Core.Logging;

namespace Syringe.Tests
{
	public class TestHelpers
	{
		public static void EnableLogging()
		{
#if DEBUG
			Log.UseConsole();
#endif
		}

		public static string ReadEmbeddedFile(string file, string namespacePath)
		{
			string resourcePath = string.Format("{0}{1}", namespacePath, file);

			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
			if (stream == null)
				throw new InvalidOperationException(string.Format("Unable to find '{0}' as an embedded resource", resourcePath));

			string textContent = "";
			using (StreamReader reader = new StreamReader(stream))
			{
				textContent = reader.ReadToEnd();
			}

			return textContent;
		}
	}
}