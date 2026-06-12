using System;
using System.IO;

namespace WindsOfMagicRestore.Utilities
{
    internal static class ModTrace
    {
        private static readonly string TracePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Mount and Blade II Bannerlord",
            "WindsOfMagicRestore_trace.txt");

        public static void Mark(string step)
        {
            try
            {
                var directory = Path.GetDirectoryName(TracePath);
                if (!string.IsNullOrEmpty(directory))
                    Directory.CreateDirectory(directory);

                File.AppendAllText(
                    TracePath,
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {step}{Environment.NewLine}");
            }
            catch
            {
            }
        }

        public static void Mark(string step, Exception ex)
        {
            Mark($"{step} | {ex.GetType().Name}: {ex.Message}");
        }
    }
}
