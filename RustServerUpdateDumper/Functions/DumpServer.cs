using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RustServerUpdateDumper.Functions
{
    public class DumpServer
    {
        public static void InitDumper(bool staging = false)
        {
            var _config = Program._config;

            var updateCommand = "+app_update 258550";
            if (staging)
                updateCommand = "+app_update 258550 -beta staging";

            var serverPath = _config.liveServerPath;
            if (staging)
                serverPath = _config.stagingServerPath;

            var serverVersion = _config.liveServerVersion;
            if (staging)
                serverVersion = _config.stagingServerVersion;

            var oldServerVersion = _config.liveServerVersion;
            if (staging)
                oldServerVersion = _config.stagingServerVersion;

            var dumpPath = _config.liveServerDumpPath;
            if (staging)
                dumpPath = _config.stagingServerDumpPath;


            Console.WriteLine(DateTime.Now + " Downloading new Rust update");

            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/C " + Path.Combine(_config.steamcmdPath, "steamcmd.exe") + " +force_install_dir " + serverPath + " +login anonymous " + updateCommand + " +quit",
                    WorkingDirectory = _config.Path
                };

                process.Start();

                process.WaitForExit();
            }

            var steamappsPath = Path.Combine(serverPath, "steamapps");

            if (!Directory.Exists(steamappsPath))
                return;

            var appManifestPath = Path.Combine(serverPath, "steamapps", "appmanifest_258550.acf");
            if (!File.Exists(appManifestPath))
                return;

            foreach (var line in File.ReadAllLines(appManifestPath))
            {
                var text = line.Trim();

                if (text.Length < 3)
                    continue;

                text = text.Replace("\"", "").Trim();
                text = text.Replace("\t\t", "-").Trim();

                var search = text.Split('-');

                if (search[0] == "buildid")
                {
                    if(staging)
                        _config.stagingServerVersion = search[1].Trim();
                    else
                        _config.liveServerVersion = search[1].Trim();

                    serverVersion = search[1].Trim();

                    break;
                }
            }

            if(oldServerVersion != serverVersion)
            {
                Console.WriteLine(DateTime.Now + " New server update detected");

                using (var process = new Process())
                {

                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/C ilspycmd -p Assembly-CSharp.dll -o " + dumpPath,
                        WorkingDirectory = Path.Combine(serverPath, "RustDedicated_Data", "Managed")
                    };

                    Console.WriteLine(DateTime.Now + " Starting dumping server");
                    process.Start();

                    process.WaitForExit();

                    Console.WriteLine(DateTime.Now + " Finished dumping server");
                }

                using (var process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/C git add . & git commit -am \"{serverVersion} - {DateTime.Now}\" & git push --force",
                        WorkingDirectory = dumpPath
                    };

                    Console.WriteLine(DateTime.Now + " Preparing to push changes to github");
                    process.Start();

                    process.WaitForExit();

                    Console.WriteLine(DateTime.Now + " Finished pushing changes to github");
                }

                File.WriteAllText("config.json", JsonConvert.SerializeObject(_config, Formatting.Indented));
            }
        }
    }
}
