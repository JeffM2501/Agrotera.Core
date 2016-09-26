using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using System.CodeDom.Compiler;

namespace Agrotera.Scripting
{
    public static class ScriptEngine
    {
        private static CodeDomProvider CSPProvider = GetCSharpCodeProvider();

        private static CodeDomProvider GetCSharpCodeProvider()
        {
            try
            {
                return new Microsoft.CSharp.CSharpCodeProvider();
            }
            catch (System.Exception /*ex*/)
            {
            }
            return null;
        }

        internal static List<ScriptInfo> Scripts = new List<ScriptInfo>();

		internal static bool ScriptDirIsLoaded(DirectoryInfo dir)
		{
			return Scripts.Find(x => x.ScriptDir != null && (x.ScriptDir.FullName.ToLower() == dir.FullName.ToLower())) != null;
		}

		internal static bool AssemblyIsLoaded(FileInfo file)
		{
			return Scripts.Find(x => x.ScriptAssembly != null && (x.ScriptAssembly.Location.ToLower() == file.FullName.ToLower())) != null;
		}

		public static void ScanFolder(string dirName)
		{
			DirectoryInfo dir = new DirectoryInfo(dirName);
			foreach (DirectoryInfo subdir in dir.GetDirectories())
			{
				if(ScriptDirIsLoaded(subdir))
					continue;

				ScriptInfo info = new ScriptInfo();
				info.Load(subdir);
				if (info.Valid)
					Scripts.Add(info);
				else if (ScriptCompileErrors != null)
				{
					ScriptCompileErrorEventArgs errors = new ScriptCompileErrorEventArgs();
					if (info.ScriptDir != null)
						errors.ScriptFolder = info.ScriptDir.FullName;
					errors.Errors = info.Errors;
					ScriptCompileErrors(info, errors);
				}
			}

			foreach (FileInfo file in dir.GetFiles("*.dll"))
			{
				if(AssemblyIsLoaded(file))
					continue;

				try
				{
					LoadAssembly(Assembly.LoadFile(file.FullName));
				}
				catch (System.Exception ex)
				{
					CatchScriptException(ex);
				}
			}	
		}

		public static void LoadAssembly(Assembly assemby)
		{
			if (assemby == null)
				return;

			ScriptInfo info = new ScriptInfo();
			info.ScriptAssembly = assemby;

			foreach (Type t in info.ScriptAssembly.GetTypes())
			{
				Type scriptInterface = t.GetInterface(typeof(IScript).FullName, false);

				if (scriptInterface != null)
				{
					IScript script = Activator.CreateInstance(t) as IScript;
					if (script != null)
						info.ScriptInterfaces.Add(script);
				}
			}

			info.Valid = info.ScriptInterfaces.Count > 0;

			Scripts.Add(info);
		}

		public static string LastInitedScript = string.Empty;

		public static void Init()
		{
			foreach(ScriptInfo script in Scripts)
			{
				if(script.Inited)
					continue;

				script.Inited = true;
				try
				{
					if (script.ScriptDir != null)
						LastInitedScript = script.ScriptDir.FullName;

					foreach (IScript entry in script.ScriptInterfaces)
						entry.InitAgroteraScript();

				}
				catch (System.Exception ex)
				{
					CatchScriptException(ex);
				}
			}
		}

		public static event EventHandler ScriptException = null;

		public class ScriptCompileErrorEventArgs : EventArgs
		{
			public string ScriptFolder = string.Empty;
			public List<string> Errors = new List<string>();
		}

		public static event EventHandler<ScriptCompileErrorEventArgs> ScriptCompileErrors = null;

		public static bool ThrowErrors = false;

		private static void CatchScriptException(Exception ex)
		{
			if(ScriptException != null)
				ScriptException(ex, EventArgs.Empty);
			else if (ThrowErrors)
				throw(ex);
		}

		internal class ScriptInfo
		{
			public DirectoryInfo ScriptDir = null;
			public Assembly ScriptAssembly = null;

			public List<IScript> ScriptInterfaces = new List<IScript>();

			public bool Valid = false;

			public bool Inited = false;

			public List<string> Errors = new List<string>();

            public void Load(DirectoryInfo dir)
            {
            if (CSPProvider == null)
                    return;

                Errors.Clear();

                ScriptDir = dir;
                List<string> scriptFilenames = new List<string>();
                foreach (FileInfo file in dir.GetFiles("*.cs"))
                    scriptFilenames.Add(file.FullName);

                CompilerParameters parameters = new CompilerParameters();
                parameters.ReferencedAssemblies.Add(Assembly.GetAssembly(typeof(ScriptInfo)).Location);

                parameters.GenerateExecutable = false;
                parameters.GenerateInMemory = true;
                parameters.IncludeDebugInformation = false;

                // #if DEBUG
                // 				parameters.IncludeDebugInformation = true;
                // #endif
                parameters.OutputAssembly = "script." + dir.Name.Replace(" ", "_");

                CompilerResults results = CSPProvider.CompileAssemblyFromFile(parameters, scriptFilenames.ToArray());

                Valid = true;
                if (results.Errors.HasErrors)
                {
                    Valid = false;

                    foreach (var error in results.Errors)
                        Errors.Add(error.ToString());
                }

                if (Valid)
                {
                    ScriptAssembly = results.CompiledAssembly;
                    foreach (Type t in ScriptAssembly.GetTypes())
                    {
                        Type scriptInterface = t.GetInterface(typeof(IScript).FullName, false);

                        if (scriptInterface != null)
                        {
                            IScript script = Activator.CreateInstance(t) as IScript;
                            if (script != null)
                                ScriptInterfaces.Add(script);
                        }
                    }

                    Valid = ScriptInterfaces.Count > 0;
                }
            }
        }
    }
}
