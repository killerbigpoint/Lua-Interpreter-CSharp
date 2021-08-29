using MunchenClient.Utils;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunchenClient.Lua
{
    internal class LuaClassWrapper
    {
        internal string className;
        internal Type classType;

        internal readonly Dictionary<string, Dictionary<int, FastMethodInfo>> functions = new Dictionary<string, Dictionary<int, FastMethodInfo>>();
    }
}
