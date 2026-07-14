namespace ZipGameImporter
{
    public static class StringHelper
    {
        public static string NoSpace(this string str)
        {
            return str.Replace(" ", "");
        }
    }
}
