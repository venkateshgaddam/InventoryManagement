namespace Carrier.CCP.Common.Utils.Extension
{
    public static class StringExtensions
    {
        public static string ReplaceFirst(this string data, string text, string search, string replace)
        {
            var pos = text.IndexOf(search);
            if (pos < 0) return text;
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }
}