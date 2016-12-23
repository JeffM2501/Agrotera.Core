using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;

using ScenarioServer.Interfaces;
using System.Diagnostics;
using System.Threading;

namespace ScenarioServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ScenarioControllerLoader loader = new ScenarioControllerLoader();
            loader.Scan(Assembly.GetExecutingAssembly());   // aways add ourselves

            var dir = ScenariosDir();
            if (dir.Exists)
                LoadScenarios(dir, loader);

            ScenarioState state = new ScenarioState(loader.GetDefaultScenario());

            state.Startup();

            while (state != null)
            {
                state.Update();
                Thread.Sleep(10);
            }
        }

        static void LoadScenarios(DirectoryInfo dir, ScenarioControllerLoader loader)
        {
            foreach (var f in dir.GetFiles("*.dll"))
                loader.Scan(Assembly.LoadFile(f.FullName));

            foreach (var d in dir.GetDirectories())
                LoadScenarios(d, loader);
        }

        static DirectoryInfo ScenariosDir()
        {
            return new DirectoryInfo(Path.Combine(GetExeDir().FullName, "scenarios"));
        }

        static DirectoryInfo GetExeDir()
        {
            return new DirectoryInfo(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
        }
    }
}
