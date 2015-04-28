namespace xServer.Core.Misc
{
    public class Update
    {
        public static string DownloadURL { get; set; }
    }

    public class VisitWebsite
    {
        public static string URL { get; set; }
        public static bool Hidden { get; set; }
    }

    public class DownloadAndExecute
    {
        public static string URL { get; set; }
        public static bool RunHidden { get; set; }
    }

    public class UploadAndExecute
    {
        public static string FilePath { get; set; }
        public static bool RunHidden { get; set; }
    }

    public class AutostartItem
    {
        public static string Name { get; set; }
        public static string Path { get; set; }
        public static int Type { get; set; }
    }
}