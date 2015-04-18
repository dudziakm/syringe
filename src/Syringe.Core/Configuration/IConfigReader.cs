using System.IO;

namespace Syringe.Core.Configuration
{
	public interface IConfigReader
	{
		Config Read(TextReader textReader);
	}
}