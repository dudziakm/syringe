using System.Text;
using Syringe.Core.Results.Writer;

namespace Syringe.Tests.Unit.StubsMocks
{
	public class ResultWriterStub : ResultWriterBase
	{
		public StringBuilder StringBuilder { get; set; }

		public ResultWriterStub()
		{
			StringBuilder = new StringBuilder();
		}

		protected override void WriteLine(string format, params object[] args)
		{
			StringBuilder.AppendFormat(format, args);
		}
	}
}