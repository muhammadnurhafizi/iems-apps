namespace IEMSApps.Classes
{
    public static class ExtensionMethods
    {
        public static string ReplaceSingleQuote(this string str)
        {
            if (str == null)
                return string.Empty;
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            return str.Replace("'", "''").Replace(@"\", @"\\");
        }
    }
}