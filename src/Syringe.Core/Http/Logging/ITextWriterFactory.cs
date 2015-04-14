using System.IO;

namespace Syringe.Core.Http.Logging
{
	public interface ITextWriterFactory
	{
		TextWriter GetWriter();
	}
}