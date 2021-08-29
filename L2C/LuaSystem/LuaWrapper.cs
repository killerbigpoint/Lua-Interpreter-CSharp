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
        private static readonly Dictionary<string, LuaClassWrapper> classes = new Dictionary<string, LuaClassWrapper>();

        internal static bool InternalFunctionExists(string className, string functionName)
        {
            if(classes.ContainsKey(className) == false)
            {
                return false;
            }

            return classes[className].functions.ContainsKey(functionName);
        }

        internal static CallbackStatus CallInternalFunction(string className, string functionName, object[] parameters)
        {
            if (InternalFunctionExists(className, functionName) == false)
            {
                return CallbackStatus.CallbackStatus_FunctionNonExistant;
            }

            if (classes[className].functions[functionName].ContainsKey(parameters.Length) == false)
            {
                return CallbackStatus.CallbackStatus_WrongParameters;
            }

            try
            {
                classes[className].functions[functionName][parameters.Length].Invoke(null, parameters);
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

                if(RegisterFunctionCallback(internalClass.Name, functionName, method) == true)
                {
                    success = true;
                }
            }

            return success;
        }

        internal static bool RegisterFunctionCallback(string className, string functionName, MethodInfo internalFunction)
        {
            if (internalFunction == null)
            {
                return false;
            }

            if (InternalFunctionExists(className, functionName) == false)
            {
                return false;
            }

            if (classes[className].functions.ContainsKey(functionName) == false)
            {
                classes[className].functions.Add(functionName, new Dictionary<int, FastMethodInfo>());
            }

            int internalFunctionParameterCount = internalFunction.GetParameters().Length;

            if (classes[className].functions[functionName].ContainsKey(internalFunctionParameterCount) == true)
            {
                return false;
            }

            classes[className].functions[functionName].Add(internalFunctionParameterCount, new FastMethodInfo(internalFunction));

            return true;
        }
    }
}
