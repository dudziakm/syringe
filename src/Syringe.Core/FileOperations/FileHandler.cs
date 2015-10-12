using System;
using System.Collections.Generic;
using System.IO;
using Syringe.Core.Configuration;
using Syringe.Core.Logging;

namespace Syringe.Core.FileOperations
{
    public class FileHandler : IFileHandler
    {
        private readonly IApplicationConfiguration _appConfig;

        public FileHandler(IApplicationConfiguration appConfig)
        {
            _appConfig = appConfig;
        }

        public FileHandler() : this(new ApplicationConfig()) { }

        public string GetFileFullPath(string fileName, string teamName)
        {
            string fullPath = Path.Combine(_appConfig.TestCasesBaseDirectory, teamName, fileName);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException("The test case path cannot be found", fileName);

            return fullPath;
        }

        public string GetFullPath(string teamName)
        {
            string fullPath = Path.Combine(_appConfig.TestCasesBaseDirectory, teamName);
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

        public IEnumerable<string> GetFileNames(string teamName)
        {
            foreach (string file in Directory.EnumerateFiles(teamName))
            {
                var fileInfo = new FileInfo(file);
                yield return fileInfo.Name;
            }
        }
    }
}
