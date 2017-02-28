using System;

namespace Extensions.GenericExtensions
{
    public static class GenericExtension
    {
        public static bool Between<T>(this T value, T minValueInclusive, T maxValueInclusive) where T : IComparable<T>
        {
            if (minValueInclusive.CompareTo(maxValueInclusive) > 0)
            {
                throw new ArgumentException("minimum value must not be greater than maximum value");
            }
            return (value.CompareTo(minValueInclusive) >= 0 && value.CompareTo(maxValueInclusive) <= 0);
        }
        public static bool CheckNull<T>(this T value)
        {
            return value.Equals(default(T));
        }
        

    }
}
