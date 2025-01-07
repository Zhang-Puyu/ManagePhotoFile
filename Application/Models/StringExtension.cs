namespace PhotoTools.Models
{
    public static class StringExtension
    {
        // 判断一个字符串是否为合法路径
        public static bool IsInvalidPath(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }
            if (path.IndexOfAny(System.IO.Path.GetInvalidPathChars()) >= 0)
            {
                return false;
            }
            return true;
        }
    }
}
