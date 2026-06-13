using System;

namespace WindsOfMagicRestore.Infrastructure
{
    internal static class ModGuard
    {
        public static void Run(string context, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                ModLog.Exception(context, ex);
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
                ModLog.Exception(context, ex);
                return fallback;
            }
        }
    }
}
