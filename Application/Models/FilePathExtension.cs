namespace PhotoTools.Application.Models
{
    public static class FilePathExtension
    {
        // 判断一个字符串是否为合法路径
        public static bool IsValidPath(this string path)
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
