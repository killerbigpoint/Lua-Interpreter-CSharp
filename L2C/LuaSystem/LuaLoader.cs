﻿using System.Collections.Generic;
using System.IO;
using System;

namespace MunchenClient.Lua
{
    internal class LuaLoader
    {
        private static readonly Dictionary<string, LuaScript> loadedLuas = new Dictionary<string, LuaScript>();

        internal static LuaScript LoadLua(string scriptName)
        {
            string tempFile = $"D:/{scriptName}.lua";

            if(loadedLuas.ContainsKey(scriptName) == true)
            {
                return loadedLuas[scriptName];
            }

            if (File.Exists(tempFile) == false)
            {
                Console.WriteLine($"Failed to load lua: {tempFile} (File doesn't exist)");

                return null;
            }

            //TODO: Integrity check - maybe some information regarding the lua

            string scriptCode = File.ReadAllText(tempFile);

            loadedLuas.Add(scriptName, new LuaScript
            {
                scriptName = scriptName,
                scriptVersion = 1,
                scriptAutoload = false,
                scriptCode = scriptCode,

                scriptFunctions = LuaAnalyzer.AnalyzeScript(scriptCode)
            });

            Console.WriteLine($"LuaScript: {tempFile} successfully loaded");

            return loadedLuas[scriptName];
        }

        internal static LuaScript GetLuaScript(string scriptName)
        {
            if(loadedLuas.ContainsKey(scriptName) == false)
            {
                return null;
            }

            return loadedLuas[scriptName];
        }
    }
}