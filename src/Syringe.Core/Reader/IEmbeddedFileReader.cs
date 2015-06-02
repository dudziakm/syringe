using System.IO;

namespace Syringe.Core.Reader
{
    public interface IEmbeddedFileReader
    {
        TextReader Get(string file);
    }
}