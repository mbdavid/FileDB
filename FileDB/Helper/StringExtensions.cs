using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Numeria.IO
{
    internal static class StringExtensions
    {
        public static byte[] ToBytes(this string str, int size)
        {
            if (string.IsNullOrEmpty(str))
                return new byte[size];

            var buffer = new byte[size];
            var strbytes = Encoding.UTF8.GetBytes(str);

            Array.Copy(strbytes, buffer, size > strbytes.Length ? strbytes.Length : size);

            return buffer;
        }
    }
}
