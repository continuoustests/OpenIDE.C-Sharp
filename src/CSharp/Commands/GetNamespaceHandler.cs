using System;
using System.IO;
using CSharp.FileSystem;
using CSharp.Responses;
using CSharp.Projects;

namespace CSharp.Commands
{
    class GetNamespaceHandler : ICommandHandler
    {
        private string _keyPath;
        private IProjectHandler _project;
        private Func<string, ProviderSettings> _getTypesProviderByLocation;

        public string Usage {
            get {
                return
                    Command+"|\"Gets default namespace for a given file\" " +
                    "   FILE|\"Path to get namespace for\" end " +
                    "end ";
            }
        }

        public string Command { get { return "get-namespace"; } }

        public GetNamespaceHandler(string keyPath, Func<string, ProviderSettings> provider)
        {
            _keyPath = keyPath;
            _getTypesProviderByLocation = provider;
            _project = new ProjectHandler();
        }

        public void Execute(IResponseWriter writer, string[] arguments)
        {
            if (arguments.Length < 1) {
                writer.Write("error|Invalid number of arguments. " +
                    "Usage: get-namespace PATH_TO_FILE");
                return;
            }

            var location = getLocation(arguments[0]);
            if (!_project.Read(location, _getTypesProviderByLocation))
                return;

            var ns = getNamespace(location, _project.Fullpath, _project.DefaultNamespace);
            writer.Write(ns);
        }

        private string getLocation(string className)
        {
            var dir = Path.GetDirectoryName(className).Trim();
            if (dir.Length == 0)
                return _keyPath;
            if (Directory.Exists(Path.Combine(_keyPath, dir)))
                return Path.Combine(_keyPath, dir);
            if (Directory.Exists(dir))
                return dir;
            
            if (!Path.IsPathRooted(dir))
                dir = Path.Combine(_keyPath, dir);
            Directory.CreateDirectory(dir);

            return dir;
        }

        private string getNamespace(string location, string project, string defaultNamespace)
        {
            var projectLocation = Path.GetDirectoryName(project);
            var relativePath = PathExtensions.GetRelativePath(projectLocation, location);
            if (relativePath.Length == 0 || relativePath.Equals(location))
                return defaultNamespace;
            return string.Format("{0}.{1}",
                defaultNamespace,
                relativePath.Replace(Path.DirectorySeparatorChar.ToString(), "."));
        }
    }
}
