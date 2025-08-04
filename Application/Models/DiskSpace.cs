using System.IO;

namespace PhotoTools.Application.Models
{
    public class DiskSpace
    {
        public static double GetUsedSpaceKB(string path)
        {
            string driveName = path.Substring(0, 2);
            DriveInfo driveInfo = new DriveInfo(driveName);
            return (driveInfo.TotalSize - driveInfo.AvailableFreeSpace) / 1024.0;
        }

        public static double GetFreeSpaceKB(string path)
        {
            string driveName = path.Substring(0, 2);
            DriveInfo driveInfo = new DriveInfo(driveName);
            return driveInfo.AvailableFreeSpace / 1024.0;
        }

        public static double GetUsedSpaceMB(string path)
        {
            return GetUsedSpaceKB(path) / 1024.0;
        }

        public static double GetFreeSpaceMB(string path)
        {
            return GetFreeSpaceKB(path) / 1024.0;
        }

        public static double GetUsedSpaceGB(string path)
        {
            return GetUsedSpaceMB(path) / 1024.0;
        }
        public static double GetFreeSpaceGB(string path)
        {
            return GetFreeSpaceMB(path) / 1024.0;
        }
    }
}
