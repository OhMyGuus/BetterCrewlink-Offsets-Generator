using System;
using System.Linq;

namespace BCL_OffsetGenerator
{
    static class Extensions
    {
        public static int GetLineIndex(this string[] arr, Predicate<string> predicate)
        {
            return arr.Select((value, index) => new { value, index = index + 1 })
               .Where(pair => predicate(pair.value))
               .Select(pair => pair.index)
               .FirstOrDefault() - 1;
        }
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}
