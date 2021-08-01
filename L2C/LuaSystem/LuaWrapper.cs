using System.Collections.Generic;
using System.Reflection;
using System;
using MunchenClient.Utils;

namespace MunchenClient.Lua
{
    internal enum CallbackStatus : short
    {
        CallbackStatus_Unknown = 0,
        CallbackStatus_Success = 1,
        CallbackStatus_FunctionNonExistant = 1,
        CallbackStatus_WrongParameters = 2,
        CallbackStatus_UnknownError = 2,
    }
    
    internal class LuaWrapper
    {
        private static readonly Dictionary<string, Dictionary<int, FastMethodInfo>> callbacks = new Dictionary<string, Dictionary<int, FastMethodInfo>>();

        internal static bool InternalFunctionExists(string functionName)
        {
            return callbacks.ContainsKey(functionName);
        }

        internal static CallbackStatus CallInternalFunction(string functionName, object[] parameters)
        {
            if (callbacks.ContainsKey(functionName) == false)
            {
                return CallbackStatus.CallbackStatus_FunctionNonExistant;
            }

            if (callbacks[functionName].ContainsKey(parameters.Length) == false)
            {
                return CallbackStatus.CallbackStatus_WrongParameters;
            }

            try
            {
                callbacks[functionName][parameters.Length].Invoke(null, parameters);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("CallInternalFunction had an error due to wrong parameters");

                return CallbackStatus.CallbackStatus_WrongParameters;
            }
            catch (Exception)
            {
                Console.WriteLine("CallInternalFunction had an error");

                return CallbackStatus.CallbackStatus_UnknownError;
            }

            return CallbackStatus.CallbackStatus_Success;
        }

        internal static bool RegisterFunctionCallback(string functionName, Type internalClass, string internalFunctionName)
        {
            bool success = false;

            foreach(MethodInfo method in internalClass.GetMethods())
            {
                if (method.Name.Equals(internalFunctionName) == false)
                {
                    continue;
                }

                if(RegisterFunctionCallback(functionName, method) == true)
                {
                    success = true;
                }
            }

            return success;
        }

        internal static bool RegisterFunctionCallback(string functionName, MethodInfo internalFunction)
        {
            if (internalFunction == null)
            {
                return false;
            }

            if (callbacks.ContainsKey(functionName) == false)
            {
                callbacks.Add(functionName, new Dictionary<int, FastMethodInfo>());
            }

            int internalFunctionParameterCount = internalFunction.GetParameters().Length;

            if (callbacks[functionName].ContainsKey(internalFunctionParameterCount) == true)
            {
                return false;
            }

            callbacks[functionName].Add(internalFunctionParameterCount, new FastMethodInfo(internalFunction));

            Console.WriteLine($"Registered callback: {functionName} | {internalFunctionParameterCount}");

            return true;
        }
    }
}
