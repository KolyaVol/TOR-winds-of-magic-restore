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
            Append(sb, "BookSpellDamage patch target", BookSpellDamagePatch.TargetMethod() != null);
            Append(sb, "ApplyGeneralDamageModifiers patch target", ApplyGeneralDamageModifiersPatch.TargetMethod() != null);
            Append(sb, "Augment buff tracking", AugmentBuffTracker.IsTrackingAvailable);
            Append(sb, "AbilityManagerMissionLogic type", TorTypes.AbilityManagerMissionLogic != null);
            Append(sb, "SpellCastSession type", TorTypes.SpellCastSession != null);
            Append(sb, "StatusEffectComponent type", TorTypes.StatusEffectComponent != null);
            Append(sb, "MCM settings loaded", WindsOfMagicRestoreSettings.Instance != null);

            sb.AppendLine("OK = resolved/working, MISSING = not found (feature disabled).");
            return sb.ToString();
        }

        private static void Append(StringBuilder sb, string name, bool ok)
        {
            sb.AppendLine($"  [{(ok ? "OK" : "MISSING")}] {name}");
        }
    }
}
