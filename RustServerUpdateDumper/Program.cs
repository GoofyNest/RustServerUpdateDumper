using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using Newtonsoft.Json;
using RustServerUpdateDumper.Functions;

namespace RustServerUpdateDumper
{
    internal class Program
    {
        public static Config _config = new Config();

        static void Main(string[] args)
        {
            _config.steamcmdPath = Path.Combine(_config.Path, _config.steamcmdPath);
            _config.liveServerPath = Path.Combine(_config.Path, _config.liveServerPath);
            _config.stagingServerPath = Path.Combine(_config.Path, _config.stagingServerPath);

            _config.liveServerDumpPath = Path.Combine(_config.liveServerPath, "dump");
            _config.stagingServerDumpPath = Path.Combine(_config.stagingServerPath, "dump");

            if (!File.Exists("config.json"))
                File.WriteAllText("config.json", JsonConvert.SerializeObject(_config, Newtonsoft.Json.Formatting.Indented));

            _config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));

            Directory.CreateDirectory(_config.steamcmdPath);
            Directory.CreateDirectory(_config.steamcmdPath);
            Directory.CreateDirectory(_config.stagingServerPath);
            Directory.CreateDirectory(_config.liveServerDumpPath);
            Directory.CreateDirectory(_config.stagingServerDumpPath);

            if (!File.Exists(Path.Combine(_config.steamcmdPath, "steamcmd.exe")))
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile("https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip", Path.Combine(_config.steamcmdPath, "steamcmd.zip"));
                    ZipFile.ExtractToDirectory(Path.Combine(_config.steamcmdPath, "steamcmd.zip"), _config.steamcmdPath);

                    File.Delete("steamcmd.zip");
                }
            }


            if(!Directory.Exists("Dependencies"))
            {
                Directory.CreateDirectory("Dependencies");

                using (WebClient client = new WebClient())
                {
                    client.DownloadFile("https://download.visualstudio.microsoft.com/download/pr/1a5fc50a-9222-4f33-8f73-3c78485a55c7/1cb55899b68fcb9d98d206ba56f28b66/dotnet-runtime-6.0.36-win-x64.exe", "Dependencies\\dotnet-runtime-6.0.36-win-x64.exe");
                    client.DownloadFile("https://github.com/git-for-windows/git/releases/download/v2.47.1.windows.1/Git-2.47.1-64-bit.exe", "Dependencies\\Git-2.47.1-64-bit.exe");
                    Console.WriteLine("After installing the Dependencies, please run dotnet tool install --global ilspycmd");
                    Console.WriteLine("dotnet tool install --global ilspycmd");
                }
            }


            File.WriteAllText("config.json", JsonConvert.SerializeObject(_config, Newtonsoft.Json.Formatting.Indented));

            DumpServer.InitDumper(true);

            Console.WriteLine(DateTime.Now + " Program finished");
            Console.ReadKey();

        }
    }
}
