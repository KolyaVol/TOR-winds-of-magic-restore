using System.Collections.Generic;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using WindsOfMagicRestore.Patches;
using WindsOfMagicRestore.Settings;

namespace WindsOfMagicRestore.Utilities
{
    public static class DiagnosticsCommand
    {
        [CommandLineFunctionality.CommandLineArgumentFunction("add", "wom")]
        public static string AddWinds(List<string> args)
        {
            if (args == null || args.Count == 0 || !float.TryParse(args[0], out var amount))
                return "Usage: wom.add <amount>";

            TorWindsApi.AddWinds(amount);
            return $"[Winds of Magic Restore] Added {amount} winds to {Hero.MainHero?.Name}.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("diagnostics", "wom")]
        public static string Diagnostics(List<string> args)
        {
            var sb = new StringBuilder();
            sb.AppendLine("[Winds of Magic Restore] TOR_Core hook diagnostics:");

            Append(sb, "AddWindsOfMagic (winds gain)", TorWindsApi.IsAvailable);
            Append(sb, "BelongsToMainParty", AgentPartyHelper.IsBelongsToMainPartyAvailable);
            Append(sb, "CreateSpellSession patch target", CreateSpellSessionPatch.TargetMethod() != null);
            Append(sb, "FinalizeSession patch target", FinalizeSessionPatch.TargetMethod() != null);
            Append(sb, "BookSpellKill patch target", BookSpellKillPatch.TargetMethod() != null);
            Append(sb, "StatusEffectComponent type", AugmentBuffTracker.IsStatusEffectComponentTypeResolved);
            Append(sb, "StatusEffect type", AugmentBuffTracker.IsStatusEffectTypeResolved);
            Append(sb, "StatusEffectComponent._currentEffects", AugmentBuffTracker.IsCurrentEffectsFieldResolved);
            Append(sb, "StatusEffect.ApplierAgent", AugmentBuffTracker.IsApplierAgentAccessorResolved);
            Append(sb, "StatusEffect.CastId", AugmentBuffTracker.IsCastIdAccessorResolved);
            Append(sb, "StatusEffect.CurrentDuration", AugmentBuffTracker.IsCurrentDurationAccessorResolved);
            Append(sb, "Agent.GetComponent<T>", AugmentBuffTracker.IsGetStatusEffectComponentResolved);

            var settingsLoaded = WindsOfMagicRestoreSettings.Instance != null;
            Append(sb, "MCM settings loaded", settingsLoaded);

            sb.AppendLine("OK = resolved/working, MISSING = not found (feature disabled).");
            return sb.ToString();
        }

        private static void Append(StringBuilder sb, string name, bool ok)
        {
            sb.AppendLine($"  [{(ok ? "OK" : "MISSING")}] {name}");
        }
    }
}
