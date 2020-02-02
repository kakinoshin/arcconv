using System;
using System.Globalization;
using System.Linq;

namespace ArcConv.Common
{
    public static class StringExtensions
    {
        /// <summary>
        /// <para>この文字列インスタンスの末尾が、</para>
        /// <para>指定したいずれかの文字列と一致するかどうかを判断します</para>
        /// </summary>
        public static bool EndsWith(this string self, params string[] values)
        {
            return values.Any(c => self.EndsWith(c));
        }

        /// <summary>
        /// <para>指定された比較オプションを使って比較した場合に、</para>
        /// <para>この文字列インスタンスの末尾が、</para>
        /// <para>指定されたいずれかの文字列と一致するかどうかを判断します</para>
        /// </summary>
        public static bool EndsWith(this string self, StringComparison comparisonType, params string[] values)
        {
            return values.Any(c => self.EndsWith(c, comparisonType));
        }

        /// <summary>
        /// <para>指定されたカルチャを使って比較した場合に、</para>
        /// <para>この文字列インスタンスの末尾が、</para>
        /// <para>指定されたいずれかの文字列と一致するかどうかを判断します</para>
        /// </summary>
        public static bool EndsWith(this string self, bool ignoreCase, CultureInfo culture, params string[] values)
        {
            return values.Any(c => self.EndsWith(c, ignoreCase, culture));
        }
    }
}
