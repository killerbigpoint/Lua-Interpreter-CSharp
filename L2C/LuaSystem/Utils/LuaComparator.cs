namespace MunchenClient.Lua.Utils
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

    internal class LuaComparator
    {
        internal static Comparator FindComparator(string statement)
        {
            //Comparator Check for "=="
            int comparatorIndex = statement.IndexOf("==");

            if (comparatorIndex != -1)
            {
                return new Comparator
                {
                    comparatorIndex = comparatorIndex,
                    comparatorType = ComparatorType.ComparatorType_EqualTo
                };
            }

            //Comparator Check for "!="
            comparatorIndex = statement.IndexOf("!=");

            if (comparatorIndex != -1)
            {
                return new Comparator
                {
                    comparatorIndex = comparatorIndex,
                    comparatorType = ComparatorType.ComparatorType_NotEqualTo
                };
            }

            //Comparator Check for "<"
            comparatorIndex = statement.IndexOf("<");

            if (comparatorIndex != -1)
            {
                return new Comparator
                {
                    comparatorIndex = comparatorIndex,
                    comparatorType = ComparatorType.ComparatorType_LessThan
                };
            }

            //Comparator Check for ">"
            comparatorIndex = statement.IndexOf(">");

            if (comparatorIndex != -1)
            {
                return new Comparator
                {
                    comparatorIndex = comparatorIndex,
                    comparatorType = ComparatorType.ComparatorType_MoreThan
                };
            }

            //Comparator Check for "<="
            comparatorIndex = statement.IndexOf("<=");

            if (comparatorIndex != -1)
            {
                return new Comparator
                {
                    comparatorIndex = comparatorIndex,
                    comparatorType = ComparatorType.ComparatorType_LessOrEqualThan
                };
            }

            //Comparator Check for ">="
            comparatorIndex = statement.IndexOf(">=");

            if (comparatorIndex != -1)
            {
                return new Comparator
                {
                    comparatorIndex = comparatorIndex,
                    comparatorType = ComparatorType.ComparatorType_MoreOrEqualThan
                };
            }

            //Last Resort
            return new Comparator
            {
                comparatorIndex = -1,
                comparatorType = ComparatorType.ComparatorType_Unknown
            };
        }
    }
}
