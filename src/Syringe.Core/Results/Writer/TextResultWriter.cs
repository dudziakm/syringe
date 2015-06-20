using System.IO;

namespace Syringe.Core.Results.Writer
{
	public class TextResultWriter : ResultWriterBase
	{
		private readonly TextWriter _textWriter;

		public TextResultWriter(TextWriter textWriter)
		{
			_textWriter = textWriter;
		}

		protected override void WriteLine(string format, params object[] args)
		{
			_textWriter.WriteLine(format, args);
		}
	}
}