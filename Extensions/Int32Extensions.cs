using System;

namespace Extensions
{
    public static class Int32Extensions
    {
        public static bool ControlBetween(this int target, int a, int b)
        {
            return target > a && target < b;
        }
        public static int GetRandomNumber(this int a)
        {
            Random rnd = new Random(a);
            return rnd.Next(1, 100000);
        }
    }
}
