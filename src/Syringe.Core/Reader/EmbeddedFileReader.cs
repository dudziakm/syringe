using System;
using System.IO;
using System.Reflection;

namespace Syringe.Core.Reader
{
    public class EmbeddedFileReader : IEmbeddedFileReader
    {
        private readonly string _namespacePath;

        public EmbeddedFileReader(string namespacePath)
        {
            _namespacePath = namespacePath;
        }

        public TextReader Get(string file)
        {
            string resourcePath = string.Format("{0}{1}", _namespacePath, file);

            if (string.IsNullOrEmpty(file))
                throw new ArgumentNullException(file, "File not specified");
            
            Stream stream = Assembly.GetCallingAssembly().GetManifestResourceStream(resourcePath);
            if (stream == null)
                throw new InvalidOperationException(string.Format("Unable to find '{0}' as an embedded resource", resourcePath));

            return new StreamReader(stream);
        }
    }
}