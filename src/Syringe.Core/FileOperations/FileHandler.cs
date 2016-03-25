using System;
using System.Collections.Generic;
using System.IO;
using Syringe.Core.Configuration;
using Syringe.Core.Logging;

namespace Syringe.Core.FileOperations
{
    public class FileHandler : IFileHandler
    {
        private readonly IConfiguration _configuration;

        public FileHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetFileFullPath(string teamName, string fileName)
        {
            string fullPath = Path.Combine(_configuration.TestCasesBaseDirectory, teamName, fileName);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException("The test case path cannot be found", fileName);

            return fullPath;
        }

        public string CreateFileFullPath(string teamName, string fileName)
        {
            return Path.Combine(_configuration.TestCasesBaseDirectory, teamName, fileName);
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public string GetTeamDirectoryFullPath(string teamName)
        {
            string fullPath = Path.Combine(_configuration.TestCasesBaseDirectory, teamName);
            if (!Directory.Exists(fullPath))
                throw new DirectoryNotFoundException("The team full path cannot be found");

            return fullPath;
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public bool WriteAllText(string path, string contents)
        {
            try
            {
                File.WriteAllText(path, contents);
                return true;
            }
            catch (Exception exception)
            {
                //todo log error
                Log.Error(exception, exception.Message);
            }

            return false;
        }

        public IEnumerable<string> GetFileNames(string fullPath)
        {
            foreach (string file in Directory.EnumerateFiles(fullPath))
            {
                var fileInfo = new FileInfo(file);
                yield return fileInfo.Name;
            }
        }

        public string CreateFilename(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException("filename");
            }

            if (!filename.EndsWith(".xml"))
            {
                filename = string.Concat(filename, ".xml");
            }

            return filename;
        }
    }
}
