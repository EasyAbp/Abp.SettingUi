﻿namespace EasyAbp.Abp.SettingUi.Extensions
{
    public static class StringExtensions
    {
        public static string DotToUnderscore(this string str)
        {
            return str.Replace('.', '_');
        }
        public static string UnderscoreToDot(this string str)
        {
            return str.Replace('_', '.');
        }
    }
}