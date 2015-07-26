using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace xClient.Core.Helper
{
    public static class FormatHelper
    {
        public static string FormatMacAddress(string macAddress)
        {
            return (macAddress.Length != 12)
                ? "00:00:00:00:00:00"
                : Regex.Replace(macAddress, "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})", "$1:$2:$3:$4:$5:$6");
        }

        public static string DriveTypeName(DriveType type)
        {
            switch (type)
            {
                case DriveType.Fixed:
                    return "Local Disk";
                case DriveType.Network:
                    return "Network Drive";
                case DriveType.Removable:
                    return "Removable Drive";
                default:
                    return type.ToString();
            }
        }

        public static string FormatScreenResolution(Rectangle resolution)
        {
            return string.Format("{0}x{1}", resolution.Width, resolution.Height);
        }
    }
}
