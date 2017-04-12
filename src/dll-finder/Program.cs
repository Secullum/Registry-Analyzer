using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;

namespace dll_finder
{
    public static class Program
    {
        private static void RunCommandLine(params string[] args)
        {
            var process = new Process();

            process.StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = string.Join(" ", args),
                UseShellExecute = false,
                WorkingDirectory = Environment.CurrentDirectory,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            process.Start();
            process.WaitForExit(100);
        }

        private static List<string> FindPaths(RegistryKey key, string dllName)
        {
            return key
                .GetSubKeyNames()
                .ToList()
                .Select(name => key.OpenSubKey(name)?.OpenSubKey("0")?.OpenSubKey("win32")?.GetValue(""))
                .Where(value => value != null)
                .Select(value => value.ToString())
                .Where(value => Path.GetFileName(value).ToLower().Contains(dllName))
                .ToList();
        }

        private static List<string> FindPaths(string dllName)
        {
            var windowsRegistry = Registry.ClassesRoot.OpenSubKey("TypeLib");

            return windowsRegistry
                .GetSubKeyNames()
                .Select(windowsRegistry.OpenSubKey)
                .Where(key => key != null)
                .SelectMany(subKey => FindPaths(subKey, dllName))
                .ToList();
        }

        private static void Verify(List<string> paths)
        {
            Console.WriteLine();

            paths.ForEach(path =>
            {
                Console.WriteLine(path);
                Console.WriteLine(File.Exists(path) ? "O arquivo existe" : "O arquivo NÃO existe");
                Console.WriteLine();
            });
        }
        
        private static void Unregistry(List<string> dlls)
        {
            Console.WriteLine();

            dlls
                .ToList()
                .ForEach(path =>
                {
                    Console.WriteLine(path);
                    Console.WriteLine("Desregistrando...");

                    if (File.Exists(path))
                    {
                        RunCommandLine("/C", "regsvr32", "/u", "/s", $"\"{path}\"", "> nul");
                        Console.WriteLine("Ok");
                    }
                    else
                    {
                        Console.WriteLine("Não é possível desregistrar um arquivo que não existe");
                    }
                
                    Console.WriteLine();
                });
        }

        private static void Delete(List<string> dlls)
        {
            Console.WriteLine();
            
            dlls
                .Where(File.Exists)
                .ToList()
                .ForEach(path =>
                {
                    Console.WriteLine(path);
                    Console.WriteLine("Excluindo...");

                    File.Delete(path);

                    Console.WriteLine("Ok");
                    Console.WriteLine();
                });
        }

        private static void Help()
        {
            Console.WriteLine();
            Console.WriteLine("Encontra as DLLs registradas no Windows");
            Console.WriteLine();
            Console.WriteLine("USO");
            Console.WriteLine("  dll-finder <nome> [/v] [/u] [/d]");
            Console.WriteLine();
            Console.WriteLine("PARÂMETROS");
            Console.WriteLine("  /v - Verifica a existência das DLLs registradas que tiverem <nome> em seu nome");
            Console.WriteLine("  /u - Desregistra as DLLs registradas que tiverem <nome> em seu nome");
            Console.WriteLine("  /d - Exclui as DLLs registradas que tiverem <nome> em seu nome");
            Console.WriteLine();
        }

        private static bool IsAdmin()
        {
            var principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        static void Main(string[] args)
        {
            if (!args.Any(arg => arg.StartsWith("/")) || args.Any(arg => arg == "/?"))
            {
                Help();

                return;
            }

            var name = args.FirstOrDefault(arg => !arg.StartsWith("/"));

            if (string.IsNullOrEmpty(name))
            {
                Help();

                return;
            }

            if (!IsAdmin())
            {
                Console.WriteLine();
                Console.WriteLine("Este programa deve ser executado com privilégios de ADMINISTRADOR");

                return;
            }

            var paths = FindPaths(name);

            if (args.Any(arg => arg == "/v"))
            {
                Verify(paths);
            }

            if (args.Any(arg => arg == "/u"))
            {
                Unregistry(paths);
            }

            if (args.Any(arg => arg == "/d"))
            {
                Delete(paths);                
            }
        }
    }
}
