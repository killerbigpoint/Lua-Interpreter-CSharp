using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2C.LuaSystem.Utils
{
    internal enum ManipulatorType : short
    {
        ManipulatorType_Unknown = 0,
        ManipulatorType_Assign = 1,
        ManipulatorType_Addition = 2,
        ManipulatorType_Subtract = 3,
        ManipulatorType_Multiply = 4,
        ManipulatorType_Division = 5,
        ManipulatorType_Increment = 6,
        ManipulatorType_Decrement = 7,
    }

    internal struct Manipulator
    {
        internal int manipulatorIndex;
        internal ManipulatorType manipulatorType;
    }

    internal class LuaManipulator
    {
        internal static Manipulator FindManipulator(string statement)
        {
            //Manipulator Check for "++"
            int comparatorIndex = statement.IndexOf("++");

            if (comparatorIndex != -1)
            {
                return new Manipulator
                {
                    manipulatorIndex = comparatorIndex,
                    manipulatorType = ManipulatorType.ManipulatorType_Increment
                };
            }

            //Manipulator Check for "--"
            comparatorIndex = statement.IndexOf("--");

            if (comparatorIndex != -1)
            {
                return new Manipulator
                {
                    manipulatorIndex = comparatorIndex,
                    manipulatorType = ManipulatorType.ManipulatorType_Decrement
                };
            }

            //Manipulator Check for "="
            comparatorIndex = statement.IndexOf("=");

            if (comparatorIndex != -1)
            {
                return new Manipulator
                {
                    manipulatorIndex = comparatorIndex,
                    manipulatorType = ManipulatorType.ManipulatorType_Assign
                };
            }

            //Manipulator Check for "+"
            comparatorIndex = statement.IndexOf("+=");

            if (comparatorIndex != -1)
            {
                return new Manipulator
                {
                    manipulatorIndex = comparatorIndex,
                    manipulatorType = ManipulatorType.ManipulatorType_Addition
                };
            }

            //Manipulator Check for "-"
            comparatorIndex = statement.IndexOf("-=");

            if (comparatorIndex != -1)
            {
                return new Manipulator
                {
                    manipulatorIndex = comparatorIndex,
                    manipulatorType = ManipulatorType.ManipulatorType_Subtract
                };
            }

            //Manipulator Check for "*"
            comparatorIndex = statement.IndexOf("*=");

            if (comparatorIndex != -1)
            {
                return new Manipulator
                {
                    manipulatorIndex = comparatorIndex,
                    manipulatorType = ManipulatorType.ManipulatorType_Multiply
                };
            }

            //Manipulator Check for "/"
            comparatorIndex = statement.IndexOf("/=");

            if (comparatorIndex != -1)
            {
                return new Manipulator
                {
                    manipulatorIndex = comparatorIndex,
                    manipulatorType = ManipulatorType.ManipulatorType_Division
                };
            }

            //Last Resort
            return new Manipulator
            {
                manipulatorIndex = -1,
                manipulatorType = ManipulatorType.ManipulatorType_Unknown
            };
        }
    }
}
