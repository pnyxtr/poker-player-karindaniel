using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.Simple
{
    public static class RankToValue
    {
        public static int Convert(string rank)
        {
            var r = rank.ToUpper();
            if (r == "2")
                return 2;
            if (r == "3")
                return 3;
            if (r == "4")
                return 4;
            if (r == "5")
                return 5;
            if (r == "6")
                return 6;
            if (r == "7")
                return 7;
            if (r == "8")
                return 8;
            if (r == "9")
                return 9;
            if (r == "T")
                return 10;
            if (r == "J")
                return 11;
            if (r == "Q")
                return 12;
            if (r == "K")
                return 13;
            if (r == "A")
                return 14;
            return 0;
        }
    }
}
