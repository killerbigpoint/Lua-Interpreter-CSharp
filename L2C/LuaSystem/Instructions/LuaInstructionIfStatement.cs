using MunchenClient.Lua.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunchenClient.Lua.Instructions
{
    internal class LuaInstructionIfStatement : LuaInstruction
    {
        internal Comparator argumentComparator;
        internal string argumentFirst;
        internal string argumentSecond;

        internal LuaFunction codeSectionFirst;
        internal LuaFunction codeSectionSecond;

        internal override void ExecuteInstruction()
        {
            if(ExecuteComparatorCode(argumentComparator.comparatorType, argumentFirst, argumentSecond) == true)
            {
                codeSectionFirst.ExecuteFunction();
            }
            else
            {
                codeSectionSecond.ExecuteFunction();
            }
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
