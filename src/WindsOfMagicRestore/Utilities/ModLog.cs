using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaleWorlds.Library;

namespace WindsOfMagicRestore.Utilities
{
    internal static class ModLog
    {
        private const int MaxRecentErrors = 15;
        private const string Prefix = "[WindsOfMagicRestore]";

        private static readonly HashSet<string> OnceKeys = new();
        private static readonly Dictionary<string, int> ErrorCounts = new();
        private static readonly List<string> RecentErrors = new();

        public static IReadOnlyDictionary<string, int> ErrorCountsSnapshot => ErrorCounts;

        public static IReadOnlyList<string> RecentErrorsSnapshot => RecentErrors;

        public static void Info(string message) => Write("INFO", message);

        public static void Warn(string message) => Write("WARN", message);

        public static void Error(string message) => Write("ERROR", message);

        public static void OnceInfo(string key, string message)
        {
            if (OnceKeys.Add(key))
                Info(message);
        }

        public static void OnceWarn(string key, string message)
        {
            if (OnceKeys.Add(key))
                Warn(message);
        }

        public static void OnceError(string key, string message)
        {
            if (OnceKeys.Add(key))
                Error(message);
        }

        public static void Exception(string context, Exception ex)
        {
            var count = ErrorCounts.TryGetValue(context, out var existing) ? existing + 1 : 1;
            ErrorCounts[context] = count;

            if (OnceKeys.Add($"{context}:exception"))
            {
                Error(FormatException(context, ex, count));
                TrackRecentError(context, ex, count);
                return;
            }

            if (count == 10 || count == 50 || count == 100 || count % 500 == 0)
                Warn($"{context} has failed {count} times — last: {ex.GetType().Name}: {ex.Message}");
        }

        public static string FormatException(Exception ex)
        {
            var sb = new StringBuilder();
            sb.Append($"{ex.GetType().Name}: {ex.Message}");

            if (ex.InnerException != null)
                sb.Append($" | Inner: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");

            var frame = FindRelevantStackFrame(ex);
            if (frame != null)
                sb.Append($" | at {frame}");

            return sb.ToString();
        }

        private static string FormatException(string context, Exception ex, int count)
        {
            var message = $"{context} failed";
            if (count > 1)
                message += $" (occurrence #{count})";

            return $"{message} — {FormatException(ex)}";
        }

        private static void TrackRecentError(string context, Exception ex, int count)
        {
            var entry = $"[{count}x] {context}: {ex.GetType().Name} — {ex.Message}";
            RecentErrors.Add(entry);

            while (RecentErrors.Count > MaxRecentErrors)
                RecentErrors.RemoveAt(0);
        }

        private static string? FindRelevantStackFrame(Exception ex)
        {
            if (string.IsNullOrEmpty(ex.StackTrace))
                return null;

            foreach (var line in ex.StackTrace.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var trimmed = line.Trim();
                if (trimmed.Contains("WindsOfMagicRestore") || trimmed.Contains("TOR_Core"))
                    return trimmed;
            }

            return ex.StackTrace
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .FirstOrDefault();
        }

        private static void Write(string level, string message)
        {
            Debug.Print($"{Prefix}[{level}] {message}");
        }
    }
}
