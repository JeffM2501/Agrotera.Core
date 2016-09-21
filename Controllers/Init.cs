using System;
using System.Collections.Generic;
using System.Reflection;

using Agrotera.Scripting;
using Agrotera.Core.Entities.Controllers;


namespace Agrotera.DefaultControllers
{
    public class Init : IScript
    {
        public void InitAgroteraScript()
        {
            // just register the default controllers
            ControllerCache.RegisterController(typeof(Default));
            ControllerCache.RegisterController(typeof(Spinner));
			ControllerCache.RegisterController(typeof(Mover));
        }
    }
}
