using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace meridian.diagram
{
    public static class PathUtils
    {
        public static string DetectSolutionPath()
        {
            int i = 0;
            string currentDirectory = Directory.GetCurrentDirectory();
            do
            {
                if (IsSolutionDirectory(currentDirectory))
                    return currentDirectory;

                currentDirectory = GetParentDirectory(currentDirectory);
                i++;
            }
            while (i < 4);

            throw new InvalidOperationException("Failed to detect solution directory");
        }

        public static string DetectProjectPath(string projectName)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            int i = 0;
            do
            {
                string candidate = Path.Combine(currentDirectory, projectName);
                if (Directory.Exists(candidate))
                    return candidate;

                currentDirectory = GetParentDirectory(currentDirectory);
                i++;
            }
            while (i < 4);

            throw new InvalidOperationException(
                string.Format("Failed to detect directory of project '{0}'", projectName));
        }

        private static bool IsSolutionDirectory(string directory)
        {
            return Directory.GetFiles(directory, "*.sln").Length > 0;
        }

        private static string GetParentDirectory(string directory)
        {
            return Path.GetDirectoryName(directory);
        }
    }
}
