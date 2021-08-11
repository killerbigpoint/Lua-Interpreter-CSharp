using MunchenClient.Lua;
using System;
using System.Collections.Generic;
using System.Linq;

namespace L2C
{
    class Program
    {
        public static void OutputToConsole(string text)
        {
            Console.WriteLine($"OutputToConsole: {text}");
        }

        public static void OutputToConsole(string text, string text2)
        {
            Console.WriteLine($"OutputToConsole 2: {text} + {text2}");
        }

        public static void OutputBoolean(bool boolean)
        {
            Console.WriteLine($"OutputBoolean: {boolean}");
        }

        public static void Main(string[] args)
        {
            /*//LuaAPI
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            LuaWrapper.RegisterFunctionCallback("console.log", typeof(Program), nameof(OutputToConsole));
            LuaWrapper.RegisterFunctionCallback("console.boolean", typeof(Program), nameof(OutputBoolean));

            stopwatch.Stop();

            //LuaLoad
            System.Diagnostics.Stopwatch stopwatch2 = new System.Diagnostics.Stopwatch();
            stopwatch2.Start();

            LuaScript hack = LuaLoader.LoadLua("hack");

            stopwatch2.Stop();

            //LuaExecute
            System.Diagnostics.Stopwatch stopwatch3 = new System.Diagnostics.Stopwatch();
            stopwatch3.Start();

            LuaInterpreter.ExecuteFunction(hack, "OnUpdate");

            stopwatch3.Stop();

            //Output profiler stats
            Console.WriteLine($"API Load Time: {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"Lua Load Time: {stopwatch2.ElapsedMilliseconds} ms");
            Console.WriteLine($"Lua Execute Time: {stopwatch3.ElapsedMilliseconds} ms");

            //Wait for input
            Console.ReadLine();*/

            //Encryption Test
            string originalString = "Zell Is Gay";
            string encryptedString = DataEncryptor.EncryptData(originalString, DataEncryptor.encryptionKey, 5);

            Console.WriteLine($"Original String: {originalString}");
            Console.WriteLine($"Encrypted String: {encryptedString}");

            Console.ReadLine();
        }
    }

    internal class DataEncryptor
    {
        internal static readonly Dictionary<char, byte> encryptionValues = new Dictionary<char, byte>
        {
            { 'a', 01 }, { 'b', 02 }, { 'c', 03 }, { 'd', 04 }, { 'e', 05 }, { 'f', 06 }, { 'g', 07 }, { 'h', 08 }, { 'i', 09 },
            { 'j', 10 }, { 'k', 11 }, { 'l', 12 }, { 'm', 13 }, { 'n', 14 }, { 'o', 15 }, { 'p', 16 }, { 'q', 17 }, { 'r', 18 },
            { 's', 19 }, { 't', 20 }, { 'u', 21 }, { 'v', 22 }, { 'w', 23 }, { 'x', 24 }, { 'y', 25 }, { 'z', 26 }, { 'A', 27 }, 
            { 'B', 28 }, { 'C', 29 }, { 'D', 30 }, { 'E', 31 }, { 'F', 32 }, { 'G', 33 }, { 'H', 34 }, { 'I', 35 }, { 'J', 36 },
            { 'K', 37 }, { 'L', 38 }, { 'M', 39 }, { 'N', 40 }, { 'O', 41 }, { 'P', 42 }, { 'Q', 43 }, { 'R', 44 }, { 'S', 45 },
            { 'T', 46 }, { 'U', 47 }, { 'V', 48 }, { 'W', 49 }, { 'X', 50 }, { 'Y', 51 }, { 'Z', 52 }, { '1', 53 }, { '2', 54 },
            { '3', 55 }, { '4', 56 }, { '5', 57 }, { '6', 58 }, { '7', 59 }, { '8', 60 }, { '9', 61 }, { '0', 62 }, { ';', 63 },
            { ':', 64 }, { ',', 65 }, { '.', 66 }, { '-', 67 }, { '_', 68 }, { '<', 69 }, { '>', 70 }, { '@', 71 }, { '£', 72 },
            { '$', 73 }, { '€', 74 }, { '{', 75 }, { '}', 76 }, { '[', 77 }, { ']', 78 }, { '|', 79 }, { '!', 80 }, { '#', 81 },
            { '¤', 82 }, { '%', 82 }, { '&', 83 }, { '/', 84 }, { '(', 85 }, { ')', 86 }, { '=', 87 }, { '?', 88 }, { '*', 89 },
            { '~', 90 }, { '^', 91 }, { '½', 92 }
        };

        internal const string encryptionKey = "PorijaGoBoom";

        internal static string EncryptData(string data, string encryptionKey, int offset)
        {
            string baseData = Base64Encode(data);

            string encryptedData = string.Empty;

            //Calculate Offset
            int totalOffset = 0;

            foreach(char character in baseData)
            {
                if(encryptionValues.ContainsKey(character) == false)
                {
                    continue;
                }

                totalOffset += encryptionValues[character];
            }

            totalOffset += offset;

            //Actually encrypt data
            foreach (char character in baseData)
            {
                if (encryptionValues.ContainsKey(character) == false)
                {
                    encryptedData += character;
                }
                else
                {
                    int currentOffset = encryptionValues[character] + totalOffset;

                    while (currentOffset > encryptionValues.Count)
                    {
                        currentOffset -= encryptionValues.Count;
                    }

                    encryptedData += encryptionValues.ElementAt(currentOffset - 1).Key;
                }
            }

            return encryptedData;
        }

        internal static string DecryptData(string data, string encryptionKey, int offset)
        {
            string decryptedData = string.Empty;

            return decryptedData;
        }

        public static string Base64Encode(string plainText)
        {
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);

            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            byte[] base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);

            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
