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
        internal static readonly Dictionary<string, LuaClassWrapper> classes = new Dictionary<string, LuaClassWrapper>();

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

                return CallbackStatus.CallbackStatus_Success;
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
        }

        internal static LuaClassWrapper RegisterClassCallback(Type internalClass)
        {
            if (classes.ContainsKey(internalClass.Name) == true)
            {
                return null;
            }

            LuaClassWrapper classWrapper = new LuaClassWrapper()
            {
                className = internalClass.Name,
                classType = internalClass
            };

            classes.Add(internalClass.Name, classWrapper);

            return classWrapper;
        }

        internal static bool RegisterFunctionCallback(LuaClassWrapper classWrapper, string internalFunctionName)
        {
            bool success = false;

            foreach (MethodInfo method in classWrapper.classType.GetMethods())
            {
                if (method.Name.Equals(internalFunctionName) == false)
                {
                    continue;
                }

                if (RegisterFunctionCallback(classWrapper.className, method, method.Name) == true)
                {
                    success = true;
                }
            }

            return success;
        }

        internal static bool RegisterFunctionCallback(LuaClassWrapper classWrapper, string internalFunctionName, string functionName)
        {
            bool success = false;

            foreach(MethodInfo method in classWrapper.classType.GetMethods())
            {
                if (method.Name.Equals(internalFunctionName) == false)
                {
                    continue;
                }

                if(RegisterFunctionCallback(classWrapper.className, method, functionName) == true)
                {
                    success = true;
                }
            }

            return success;
        }

        internal static bool RegisterFunctionCallback(string className, MethodInfo internalFunction, string functionName)
        {
            if (internalFunction == null)
            {
                return false;
            }

            if (InternalFunctionExists(className, functionName) == true)
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
