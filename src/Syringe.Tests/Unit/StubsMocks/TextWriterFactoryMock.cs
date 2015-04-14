using System.IO;
using System.Text;
using Syringe.Core.Http;
using Syringe.Core.Http.Logging;

namespace Syringe.Tests.Unit.StubsMocks
{
	public class TextWriterFactoryMock : ITextWriterFactory
	{
		private readonly StringBuilder _stringBuilder;

		public TextWriterFactoryMock(StringBuilder stringBuilder)
		{
			_stringBuilder = stringBuilder;
		}

		public TextWriter GetWriter()
		{
			return new StringWriter(_stringBuilder);
		}
	}
}