using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RustServerUpdateDumper
{
    public class Config
    {
        public string Path { get; set; } = "C:\\";

        public string steamcmdPath { get; set; } = "steamcmd";

        //                                                              \\RustDedicated_Data\\Managed
        public string liveServerPath { get; set; } = "Rust-Servers\\live";
        public string stagingServerPath { get; set; } = "Rust-Servers\\staging";

        public string liveServerDumpPath { get; set; } = "";
        public string stagingServerDumpPath { get; set; } = "";

        public string liveServerVersion { get; set; } = "0";
        public string stagingServerVersion { get; set; } = "0";
    }
}