using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunchenClient.Lua.Analyzer
{
    internal class LuaAnalyzer
    {
        internal static Dictionary<string, LuaFunction> AnalyzeScript(string script)
        {
            Dictionary<string, LuaFunction> functions = new Dictionary<string, LuaFunction>();

            for (int i = 0; i < script.Length; i++)
            {
                LuaAnalyzerFunction.CheckForFunction(script, i, ref functions);
            }

            return functions;
        }
    }
}
