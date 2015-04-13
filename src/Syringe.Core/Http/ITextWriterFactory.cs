using System.IO;

namespace Syringe.Core.Http
{
	public interface ITextWriterFactory
	{
		TextWriter GetWriter();
	}
}