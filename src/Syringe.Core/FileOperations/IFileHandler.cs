using System.Collections.Generic;

namespace Syringe.Core.FileOperations
{
    public interface IFileHandler
    {
        string GetFullPath(string fileName, string teamName);
        string GetFullPath(string teamName);
        string ReadAllText(string path);
        bool WriteAllText(string path, string contents);
        IEnumerable<string> GetFileNames(string teamName);
    }
}