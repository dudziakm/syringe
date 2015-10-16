using System.Collections.Generic;

namespace Syringe.Core.FileOperations
{
    public interface IFileHandler
    {
        string GetFileFullPath(string fileName, string teamName);
        string GetFullPath(string teamName);
        string ReadAllText(string path);
        bool WriteAllText(string path, string contents);
        IEnumerable<string> GetFileNames(string teamName);
        bool FileExists(string filePath);
        string CreateFileFullPath(string fileName, string teamName);
        string CreateFilename(string filename);
    }
}