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

            Stopwatch timer = new Stopwatch();
            timer.Start();
            double last = timer.ElapsedMilliseconds * 0.001;

            state.Startup(last);

            var hauler1 = state.MapItems.GetByName("Cargo Transport 1");

            while (state != null)
            {
                double now = timer.ElapsedMilliseconds * 0.001;

                state.Update(now);

                if (hauler1 != null)
                {
                    Console.WriteLine(string.Format("Hauler 1 {0}, {1}, {2}", hauler1.Position.X, hauler1.Position.Y, hauler1.Position.Z));
                }

                Thread.Sleep(100);
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
