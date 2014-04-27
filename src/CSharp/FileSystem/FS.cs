using System;
using System.IO;
using System.Diagnostics;
namespace CSharp.FileSystem
{
	public class FS : IFS
	{
        public static string GetTempFilePath()
        {
            var tmpfile = Path.GetTempFileName();
            if (OS.IsOSX)
                tmpfile = Path.Combine("/tmp", Path.GetFileName(tmpfile));
            return tmpfile;
        }

        public static string GetTempDir()
        {
            if (OS.IsOSX)
                return "/tmp";
            return Path.GetTempPath();
        }

		public string[] GetFiles(string path, string searchPattern)
        {
            return Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories);
        }

		public string[] ReadLines(string path)
		{
			return File.ReadAllLines(path);
		}
		
        public string ReadFileAsText(string path)
        {
            using (var reader = new StreamReader(path))
            {
                return reader.ReadToEnd();
            }
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }
		
		public bool FileExists(string file)
		{
			return File.Exists(file);
		}
		
		public void WriteAllText(string file, string text)
		{
			File.WriteAllText(file, text);
		}
		
		public void DeleteFile(string file)
		{
			File.Delete(file);
		}
	}

    static class OS
    {
        private static bool? _isWindows;
        private static bool? _isUnix;
        private static bool? _isOSX;

        public static bool IsWindows {
            get {
                if (_isWindows == null) {
                    _isWindows = 
                        Environment.OSVersion.Platform == PlatformID.Win32S ||
                        Environment.OSVersion.Platform == PlatformID.Win32NT ||
                        Environment.OSVersion.Platform == PlatformID.Win32Windows ||
                        Environment.OSVersion.Platform == PlatformID.WinCE ||
                        Environment.OSVersion.Platform == PlatformID.Xbox;
                }
                return (bool) _isWindows;
            }
        }

        public static bool IsPosix {
            get {
                return IsUnix || IsOSX;
            }
        }

        public static bool IsUnix {
            get {
                if (_isUnix == null)
                    setUnixAndLinux();
                return (bool) _isUnix;
            }
        }

        public static bool IsOSX {
            get {
                if (_isOSX == null)
                    setUnixAndLinux();
                return (bool) _isOSX;
            }
        }

        private static void setUnixAndLinux()
        {
            try
            {
                if (IsWindows) {
                    _isOSX = false;
                    _isUnix = false;
                } else  {
                    var process = new Process
                                      {
                                          StartInfo =
                                              new ProcessStartInfo("uname", "-a")
                                                  {
                                                      RedirectStandardOutput = true,
                                                      WindowStyle = ProcessWindowStyle.Hidden,
                                                      UseShellExecute = false,
                                                      CreateNoWindow = true
                                                  }
                                      };

                    process.Start();
                    var output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    _isOSX = output.Contains("Darwin Kernel");
                    _isUnix = !_isOSX;
                }
            }
            catch
            {
                _isOSX = false;
                _isUnix = false;
            }
        }
    }
}

