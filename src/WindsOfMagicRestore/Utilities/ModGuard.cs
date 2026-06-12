using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace WindsOfMagicRestore.Utilities
{
    internal static class ModGuard
    {
        private static readonly HashSet<string> LoggedErrors = new();

        public static void Run(string context, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                LogOnce(context, ex);
            }
        }

        public static T? Run<T>(string context, Func<T> func, T? fallback = default)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                LogOnce(context, ex);
                return fallback;
            }
        }

        private static void LogOnce(string context, Exception ex)
        {
            if (!LoggedErrors.Add(context))
                return;

            Debug.Print($"[WindsOfMagicRestore] {context}: {ex.GetType().Name} - {ex.Message}");
        }
    }
}
