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
        }
        
        private List<RegistryKey> FindKeysByTerm(RegistryKey baseKey, string term)
        {
            return baseKey
                .GetSubKeyNames()
                .ToList()
                .Select(name => baseKey.OpenSubKey(name)?.OpenSubKey("0")?.OpenSubKey("win32"))
                .Where(key => key != null)
                .Where(key => key.GetValue("") != null)
                .Where(key => !string.IsNullOrEmpty(key.GetValue("").ToString()))
                .Where(key =>
                {
                    var path = key.GetValue("").ToString();

                    try
                    {
                        return Path.GetFileName(path).ToLower().Contains(term);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                })
                .ToList();
        }

        internal List<RegistryEntry> Search(string term)
        {
            var windowsRegistry = Registry.ClassesRoot.OpenSubKey("TypeLib");

            return windowsRegistry
                .GetSubKeyNames()
                .Select(windowsRegistry.OpenSubKey)
                .Where(key => key != null)
                .SelectMany(subKey => FindKeysByTerm(subKey, term.ToLower()))
                .Select(key => new RegistryEntry
                {
                    Key = key.Name.Substring(key.Name.IndexOf("{"), 38),
                    Path = key.GetValue("").ToString()
                })
                .ToList();
        }
        
        internal void Unregistry(RegistryEntry entry)
        {
            var process = new Process();

            process.StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C regsvr32 /u /s \"{entry.Path}\" > nul",
                WorkingDirectory = Environment.CurrentDirectory,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            process.Start();
            process.WaitForExit(100);
        }
    }
}
