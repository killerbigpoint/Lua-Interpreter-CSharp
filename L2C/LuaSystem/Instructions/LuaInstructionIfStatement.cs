using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunchenClient.Lua.Instructions
{
    internal enum ComparatorType : short
    {
        ComparatorType_Unknown = 0,
        ComparatorType_EqualTo = 1,
        ComparatorType_NotEqualTo = 2,
        ComparatorType_LessThan = 3,
        ComparatorType_MoreThan = 4,
        ComparatorType_MoreOrEqualThan = 5,
        ComparatorType_LessOrEqualThan = 6,
    }

    internal struct Comparator
    {
        internal int comparatorIndex;
        internal ComparatorType comparatorType;
    }

    internal class LuaInstructionIfStatement : LuaInstruction
    {
        internal Comparator argumentComparator;
        internal string argumentFirst;
        internal string argumentSecond;

        internal LuaFunction yes;
        internal LuaFunction no;

        internal override void ExecuteInstruction()
        {
            
        }

        private static bool ExecuteComparatorCode(ComparatorType comparator, string argumentFirst, string argumentSecond)
        {
            switch (comparator)
            {
                case ComparatorType.ComparatorType_EqualTo:
                {
                    return argumentFirst == argumentSecond;
                }

                case ComparatorType.ComparatorType_NotEqualTo:
                {
                    return argumentFirst != argumentSecond;
                }

                case ComparatorType.ComparatorType_LessThan:
                {
                    if (float.TryParse(argumentFirst, out float argumentFirstConverted) == false || float.TryParse(argumentSecond, out float argumentSecondConverted) == false)
                    {
                        return false;
                    }

                    return argumentFirstConverted < argumentSecondConverted;
                }

                case ComparatorType.ComparatorType_MoreThan:
                {
                    if (float.TryParse(argumentFirst, out float argumentFirstConverted) == false || float.TryParse(argumentSecond, out float argumentSecondConverted) == false)
                    {
                        return false;
                    }

                    return argumentFirstConverted > argumentSecondConverted;
                }

                case ComparatorType.ComparatorType_MoreOrEqualThan:
                {
                    if (float.TryParse(argumentFirst, out float argumentFirstConverted) == false || float.TryParse(argumentSecond, out float argumentSecondConverted) == false)
                    {
                        return false;
                    }

                    return argumentFirstConverted >= argumentSecondConverted;
                }

                case ComparatorType.ComparatorType_LessOrEqualThan:
                {
                    if (float.TryParse(argumentFirst, out float argumentFirstConverted) == false || float.TryParse(argumentSecond, out float argumentSecondConverted) == false)
                    {
                        return false;
                    }

                    return argumentFirstConverted <= argumentSecondConverted;
                }

                default:
                {
                    return false;
                }
            }
        }
    }
}
