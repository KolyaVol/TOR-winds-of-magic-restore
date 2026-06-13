using System.Collections.Generic;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace WindsOfMagicRestore.Infrastructure
{
    public static class DiagnosticsCommand
    {
        [CommandLineFunctionality.CommandLineArgumentFunction("add", "wom")]
        public static string AddWinds(List<string> args)
        {
            if (args == null || args.Count == 0 || !float.TryParse(args[0], out var amount))
                return "Usage: wom.add <amount>";

            if (!Integration.TorWindsApi.IsAvailable)
                return "[Winds of Magic Restore] Cannot add winds — TOR wind gain API not found. Run wom.diagnostics.";

            Integration.TorWindsApi.AddWinds(amount);
            return $"[Winds of Magic Restore] Added {amount} winds to {Hero.MainHero?.Name}.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("diagnostics", "wom")]
        public static string Diagnostics(List<string> args)
        {
            var sb = new StringBuilder();
            ModDiagnostics.AppendFullReport(sb);
            return sb.ToString();
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("export", "wom")]
        public static string Export(List<string> args)
        {
            ModDiagnostics.ExportReportToFile();
            return $"[Winds of Magic Restore] Diagnostics exported to {ModDiagnostics.GetDiagnosticsFilePath()}";
        }
    }
}
