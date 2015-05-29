namespace xServer.Core.Misc
{
    public static class Update
    {
        public static bool UseDownload { get; set; }
        public static string UploadPath { get; set; }
        public static string DownloadURL { get; set; }
    }

    public static class VisitWebsite
    {
        public static string URL { get; set; }
        public static bool Hidden { get; set; }
    }

    public static class DownloadAndExecute
    {
        public static string URL { get; set; }
        public static bool RunHidden { get; set; }
    }

    public static class UploadAndExecute
    {
        public static string FilePath { get; set; }
        public static bool RunHidden { get; set; }
    }

    public static class AutostartItem
    {
        public static string Name { get; set; }
        public static string Path { get; set; }
        public static int Type { get; set; }
    }

    public static class MessageBoxData
    {
        public static string Caption { get; set; }
        public static string Text { get; set; }
        public static string Button { get; set; }
        public static string Icon { get; set; }
    }
}