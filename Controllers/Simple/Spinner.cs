﻿using System;
using System.Collections.Generic;

using Agrotera.Core;
using Agrotera.DefaultControllers.SubSystems;

namespace Agrotera.DefaultControllers.Simple
{
    public class Spinner : SubSystemController
    {
        public override int Version {get{return 1;}}

        public static readonly string SpeedCommand = "Speed";

        double SpinSpeed = 1;
        public override void AddArgument(string arg, string value)
        {
            if (arg == SpeedCommand)
                double.TryParse(value, out SpinSpeed);
        }

        public override void UpdateEntity(Tick tick, Entity entity)
        {
			entity.Rotation += SpinSpeed * tick.Delta;
			entity.Update(tick);
        }
    }
}
