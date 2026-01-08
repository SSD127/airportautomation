using System.IO;

namespace AirportAutomation.Core.Helpers
{
    public static class CommonFunction
    {
        public static string GetReportDirectoryPath()
        {
            string basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AirportAutomation", "Reports");
            Directory.CreateDirectory(basePath);
            return basePath;
        }

        public static string GetTemplateDirectoryPath()
        {
            string basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AirportAutomation", "Templates");
            Directory.CreateDirectory(basePath);
            return basePath;
        }
    }
}

