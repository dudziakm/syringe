using System.Collections.Generic;

namespace Syringe.Core.IO
{
    public interface IFileHandler
    {
        string GetFileFullPath(string teamName, string fileName);
        string GetTeamDirectoryFullPath(string teamName);
        string ReadAllText(string path);
        bool WriteAllText(string path, string contents);
        IEnumerable<string> GetFileNames(string fullPath);
        bool FileExists(string filePath);
        string CreateFileFullPath(string teamName, string fileName);
        string CreateFilename(string filename);
    }
}