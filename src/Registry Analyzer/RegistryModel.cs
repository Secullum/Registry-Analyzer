using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Registry_Analyzer
{
    internal class RegistryModel
    {
        internal struct RegistryEntry
        {
            internal string Path;
            internal string Key;
            internal bool Exists;
        }
        
        private List<RegistryEntry> FindPaths(RegistryKey baseKey, string term)
        {
            return baseKey
                .GetSubKeyNames()
                .ToList()
                .Select(name => baseKey.OpenSubKey(name)?.OpenSubKey("0")?.OpenSubKey("win32"))
                .Where(key => key != null && key.GetValue("") != null && !string.IsNullOrEmpty(key.GetValue("").ToString()))
                .Select(key => new RegistryEntry
                {
                    Key = key.Name.Substring(key.Name.IndexOf("{"), 38),
                    Path = key.GetValue("").ToString(),
                    Exists = File.Exists(key.GetValue("").ToString())
                })
                .Where(entry => Path.GetFileName(entry.Path).ToLower().Contains(term))
                .ToList();
        }

        private List<RegistryEntry> FindPaths(string term)
        {
            var windowsRegistry = Registry.ClassesRoot.OpenSubKey("TypeLib");

            return windowsRegistry
                .GetSubKeyNames()
                .Select(windowsRegistry.OpenSubKey)
                .Where(key => key != null)
                .SelectMany(subKey => FindPaths(subKey, term))
                .ToList();
        }

        internal List<RegistryEntry> Search(string term)
        {
            return FindPaths(term.ToLower());
        }
        
        internal void Unregistry(RegistryEntry entry)
        {
            var process = new Process();

            process.StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C regsvr32 /u /s \"{entry.Path}\" > nul",
                UseShellExecute = false,
                WorkingDirectory = Environment.CurrentDirectory,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            process.Start();
            process.WaitForExit(100);
        }
    }
}
