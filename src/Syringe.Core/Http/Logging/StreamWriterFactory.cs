using System.IO;

namespace Syringe.Core.Http.Logging
{
	public class StreamWriterFactory : ITextWriterFactory
	{
		private readonly string _filename;

		public StreamWriterFactory(string filename)
		{
			_filename = filename;
		}

		public TextWriter GetWriter()
		{
			return new StreamWriter(new FileStream(_filename, FileMode.OpenOrCreate, FileAccess.Write));
		}
	}
}