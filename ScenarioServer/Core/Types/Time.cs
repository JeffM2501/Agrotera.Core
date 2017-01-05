using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Core.Types
{
    public static class Timer
    {
        private static readonly bool FixedTimeUpdates = true; 

        private static double LastTime = 0;

        private static Stopwatch Watch = new Stopwatch();

        public static double Now { get; private set; }
        public static double Delta { get; private set; }


        static Timer()
        {
            LastTime = 0;

            if (!FixedTimeUpdates)
                Watch.Start();

            Advance();
        }

        public static void Advance()
        {
            if (FixedTimeUpdates)
                Now = LastTime + 0.05;        // fixed 20 updates a second
            else
                Now = Watch.ElapsedMilliseconds * 0.001;

            Delta = Now - LastTime;
            LastTime = Now;
        }
    }
}
