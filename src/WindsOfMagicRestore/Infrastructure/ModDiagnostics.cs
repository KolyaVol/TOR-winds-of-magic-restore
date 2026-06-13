using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Domain;
using WindsOfMagicRestore.Integration;
using WindsOfMagicRestore.Patches;
using WindsOfMagicRestore.Settings;

namespace WindsOfMagicRestore.Infrastructure
{
    internal static class ModDiagnostics
    {
        private static bool _startupLogged;
        private static bool _battleWarningShown;
        private static List<string>? _cachedIntegrationIssues;

        public static void LogStartupReport()
        {
            if (_startupLogged)
                return;

            _startupLogged = true;
            InvalidateIntegrationCache();

            ModLog.Info($"Loaded {DescribeAssembly(typeof(ModLog).Assembly)}");
            ModLog.Info($"TOR_Core: {DescribeLoadedAssembly("TOR_Core") ?? "NOT LOADED — enable The Old Realms above this mod"}");
            ModLog.Info($"Harmony: {DescribeLoadedAssembly("0Harmony") ?? DescribeLoadedAssembly("Harmony") ?? "not found in AppDomain"}");
            ModLog.Info($"TaleWorlds.MountAndBlade: {DescribeLoadedAssembly("TaleWorlds.MountAndBlade") ?? "not found"}");

            foreach (var issue in CollectIntegrationIssues())
                ModLog.Warn(issue);
        }

        public static void InvalidateIntegrationCache()
        {
            _cachedIntegrationIssues = null;
        }

        public static void LogPatchResults(string scope, int applied, int total)
        {
            if (applied == total)
            {
                ModLog.Info($"Harmony {scope} patches applied: {applied}/{total}");
                return;
            }

            ModLog.Warn($"Harmony {scope} patches applied: {applied}/{total} — some in-battle rewards are disabled");
            ModLog.Warn("Open Mod Options → Winds of Magic Restore → Diagnostics, or check the game log file");
        }

        public static int IntegrationIssueCount => CollectIntegrationIssues().Count;

        public static string GetBriefSummary()
        {
            var issues = CollectIntegrationIssues();
            if (issues.Count == 0)
                return "All hooks OK — TOR_Core looks compatible.";

            var summary = $"{issues.Count} issue(s): {issues[0]}";
            if (issues.Count > 1)
                summary += $" (+{issues.Count - 1} more — use wom.export)";

            return summary;
        }

        public static string GetGameLogPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Mount and Blade II Bannerlord",
                "logs");
        }

        public static string GetDiagnosticsFilePath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Mount and Blade II Bannerlord",
                "WindsOfMagicRestore_diagnostics.txt");
        }

        public static void ExportReportToFile()
        {
            ModGuard.Run("ExportDiagnostics", () =>
            {
                var sb = new StringBuilder();
                AppendFullReport(sb);

                var path = GetDiagnosticsFilePath();
                var directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory))
                    Directory.CreateDirectory(directory);

                File.WriteAllText(path, sb.ToString());

                ModLog.Info($"Diagnostics exported to {path}");
                InformationManager.DisplayMessage(new InformationMessage(
                    "Winds of Magic Restore: diagnostics saved to Documents\\Mount and Blade II Bannerlord\\WindsOfMagicRestore_diagnostics.txt",
                    Colors.Green));
            });
        }

        public static void TryShowBattleWarning()
        {
            if (_battleWarningShown)
                return;

            if (WindsOfMagicRestoreSettings.Instance?.ShowBattleDiagnosticsWarning == false)
                return;

            var issueCount = IntegrationIssueCount;
            if (issueCount == 0)
                return;

            _battleWarningShown = true;
            InformationManager.DisplayMessage(new InformationMessage(
                $"Winds of Magic Restore: {issueCount} compatibility issue(s). See Mod Options → Diagnostics.",
                Colors.Yellow));
        }

        public static string GetPatchHint(string patchName)
        {
            return patchName switch
            {
                "FinalizeSession" => MissingHint(
                    TorTypes.AbilityManagerMissionLogic,
                    TorTypes.SpellCastSession,
                    "Heal-end rewards need AbilityManagerMissionLogic.FinalizeSession(SpellCastSession)."),

                "CreateSpellSession" => MissingHint(
                    TorTypes.AbilityManagerMissionLogic,
                    TorTypes.AbilityTemplate,
                    "Augment/heal tracking needs AbilityManagerMissionLogic.CreateSpellSession(Agent, AbilityTemplate)."),

                "BookSpellKill" => TorTypes.AbilityManagerMissionLogic == null
                    ? "TOR_Core ability logic type missing — spell/DOT kill rewards disabled."
                    : $"Expected BookSpellKill(int, Agent) on {TorTypes.AbilityManagerMissionLogic.FullName}. TOR may have renamed it.",

                "BookSpellDamage" => TorTypes.AbilityManagerMissionLogic == null
                    ? "TOR_Core ability logic type missing — spell damage rewards disabled."
                    : $"Expected BookSpellDamage(int, Agent, int, ...). Found: {DescribeBookSpellDamageSignatures()}",

                "ApplyGeneralDamageModifiers" => MissingHint(
                    TorTypes.TorAgentApplyDamageModel,
                    TorTypes.AttackInformation,
                    "Melee/ranged damage rewards need TORAgentApplyDamageModel.ApplyGeneralDamageModifiers."),

                _ => "Check TOR_Core version and load order.",
            };
        }

        public static void AppendFullReport(StringBuilder sb)
        {
            sb.AppendLine("[Winds of Magic Restore] Diagnostics");
            sb.AppendLine();

            sb.AppendLine("== Versions ==");
            AppendLine(sb, "This mod", DescribeAssembly(typeof(ModLog).Assembly));
            AppendLine(sb, "TOR_Core", DescribeLoadedAssembly("TOR_Core") ?? "NOT LOADED");
            AppendLine(sb, "Harmony", DescribeLoadedAssembly("0Harmony") ?? DescribeLoadedAssembly("Harmony") ?? "not found");
            AppendLine(sb, "TaleWorlds.MountAndBlade", DescribeLoadedAssembly("TaleWorlds.MountAndBlade") ?? "not found");
            AppendLine(sb, "TaleWorlds.Core", DescribeLoadedAssembly("TaleWorlds.Core") ?? "not found");
            sb.AppendLine();

            sb.AppendLine("== Reward features ==");
            AppendFeature(sb, "Winds gain API", TorWindsApi.IsAvailable, "Kill/heal/damage/passive rewards cannot grant winds");
            AppendFeature(sb, "BelongsToMainParty", Battle.AgentPartyHelper.IsBelongsToMainPartyAvailable, "Augment-kill party check falls back to team comparison");
            AppendFeature(sb, "Augment buff tracking", AugmentBuffTracker.IsTrackingAvailable, "Buffed-unit kill rewards disabled");
            AppendFeature(sb, "MCM settings", WindsOfMagicRestoreSettings.Instance != null, "Settings unavailable — using defaults may fail");
            sb.AppendLine();

            sb.AppendLine("== Harmony patch targets ==");
            AppendPatch(sb, "FinalizeSession", FinalizeSessionPatch.TargetMethod(), "Heal-end rewards");
            AppendPatch(sb, "CreateSpellSession", CreateSpellSessionPatch.TargetMethod(), "Augment/heal cast registration");
            AppendPatch(sb, "BookSpellKill", BookSpellKillPatch.TargetMethod(), "Spell/DOT/area kill rewards");
            AppendPatch(sb, "BookSpellDamage", BookSpellDamagePatch.TargetMethod(), "Spell damage rewards");
            AppendPatch(sb, "ApplyGeneralDamageModifiers", ApplyGeneralDamageModifiersPatch.TargetMethod(), "Melee/ranged damage rewards");
            sb.AppendLine();

            sb.AppendLine("== TOR types & members ==");
            AppendType(sb, "AbilityManagerMissionLogic", TorTypes.AbilityManagerMissionLogic);
            AppendType(sb, "AbilityTemplate", TorTypes.AbilityTemplate);
            AppendType(sb, "SpellCastSession", TorTypes.SpellCastSession);
            AppendType(sb, "StatusEffectComponent", TorTypes.StatusEffectComponent);
            AppendType(sb, "StatusEffect", TorTypes.StatusEffect);
            AppendType(sb, "TORAgentApplyDamageModel", TorTypes.TorAgentApplyDamageModel);
            AppendType(sb, "AttackInformation", TorTypes.AttackInformation);
            AppendMember(sb, "HeroExtensions.AddWindsOfMagic", TorTypes.AddWindsOfMagic);
            AppendMember(sb, "HeroExtensions.AddCustomResource", TorTypes.AddCustomResource);
            AppendMember(sb, "AgentExtensions.BelongsToMainParty", TorTypes.BelongsToMainParty);
            AppendMember(sb, "SpellCastSession.TotalHealingDone", TorTypes.SessionTotalHealingDone);
            AppendMember(sb, "SpellCastSession.Caster", TorTypes.SessionCaster);
            AppendMember(sb, "SpellCastSession.CasterHero", TorTypes.SessionCasterHero);
            AppendMember(sb, "SpellCastSession.CastID", TorTypes.SessionCastId);
            AppendMember(sb, "AbilityManager._activeSpellSessions", TorTypes.ActiveSpellSessionsField);
            AppendMember(sb, "AbilityManager._pendingCollectSessions", TorTypes.PendingCollectSessionsField);
            AppendMember(sb, "StatusEffectComponent._currentEffects", TorTypes.StatusEffectCurrentEffectsField);
            sb.AppendLine();

            var issues = CollectIntegrationIssues();
            if (issues.Count > 0)
            {
                sb.AppendLine("== Likely causes ==");
                foreach (var issue in issues)
                    sb.AppendLine($"  - {issue}");
                sb.AppendLine();
            }

            if (TorTypes.AbilityManagerMissionLogic != null && BookSpellDamagePatch.TargetMethod() == null)
            {
                sb.AppendLine("== BookSpellDamage signatures in loaded TOR ==");
                sb.AppendLine($"  {DescribeBookSpellDamageSignatures()}");
                sb.AppendLine();
            }

            var errorCounts = ModLog.ErrorCountsSnapshot;
            if (errorCounts.Count > 0)
            {
                sb.AppendLine("== Runtime errors since load ==");
                foreach (var entry in errorCounts.OrderByDescending(kv => kv.Value))
                    sb.AppendLine($"  {entry.Key}: {entry.Value}x");
                sb.AppendLine();
            }

            var recent = ModLog.RecentErrorsSnapshot;
            if (recent.Count > 0)
            {
                sb.AppendLine("== Recent error samples ==");
                foreach (var entry in recent)
                    sb.AppendLine($"  {entry}");
                sb.AppendLine();
            }

            sb.AppendLine("OK = resolved, MISSING = hook/type not found (feature disabled).");
            sb.AppendLine("If TOR_Core was updated, compare versions above with a known-good TOR build.");
        }

        private static List<string> CollectIntegrationIssues()
        {
            if (_cachedIntegrationIssues != null)
                return _cachedIntegrationIssues;

            var issues = new List<string>();

            if (DescribeLoadedAssembly("TOR_Core") == null)
            {
                issues.Add("TOR_Core assembly is not loaded. Enable The Old Realms and place it above WindsOfMagicRestore.");
                _cachedIntegrationIssues = issues;
                return issues;
            }

            if (!TorWindsApi.IsAvailable)
                issues.Add("Neither AddWindsOfMagic nor AddCustomResource found — no winds can be granted (TOR API changed?).");

            if (TorTypes.AbilityManagerMissionLogic == null)
                issues.Add("AbilityManagerMissionLogic type missing — most spell hooks will fail.");

            if (BookSpellKillPatch.TargetMethod() == null)
                issues.Add("BookSpellKill not found — spell, DOT, and bombardment kill rewards disabled.");

            if (BookSpellDamagePatch.TargetMethod() == null)
                issues.Add($"BookSpellDamage not found — spell damage rewards disabled. {DescribeBookSpellDamageSignatures()}");

            if (FinalizeSessionPatch.TargetMethod() == null)
                issues.Add("FinalizeSession patch target missing — heal-end rewards disabled.");

            if (CreateSpellSessionPatch.TargetMethod() == null)
                issues.Add("CreateSpellSession patch target missing — augment/heal cast tracking disabled.");

            if (!AugmentBuffTracker.IsTrackingAvailable)
                issues.Add("Status-effect reflection incomplete — augment-kill rewards may not detect active buffs.");

            if (ApplyGeneralDamageModifiersPatch.TargetMethod() == null)
                issues.Add("ApplyGeneralDamageModifiers missing — melee/ranged damage rewards disabled.");

            if (WindsOfMagicRestoreSettings.Instance == null)
                issues.Add("MCM settings not loaded. Enable Bannerlord.MBOptionScreen above this mod.");

            _cachedIntegrationIssues = issues;
            return issues;
        }

        public static string DescribeBookSpellDamageSignatures()
        {
            if (TorTypes.AbilityManagerMissionLogic == null)
                return "AbilityManagerMissionLogic not loaded";

            var methods = TorTypes.AbilityManagerMissionLogic
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.Name == "BookSpellDamage")
                .ToList();

            if (methods.Count == 0)
                return "no BookSpellDamage method on loaded TOR_Core (API likely renamed)";

            return string.Join(" | ", methods.Select(DescribeMethod));
        }

        private static string MissingHint(Type? first, Type? second, string detail)
        {
            if (first == null || second == null)
                return detail;

            return detail;
        }

        private static void AppendLine(StringBuilder sb, string label, string value)
        {
            sb.AppendLine($"  {label}: {value}");
        }

        private static void AppendFeature(StringBuilder sb, string name, bool ok, string impact)
        {
            sb.AppendLine($"  [{(ok ? "OK" : "MISSING")}] {name}{(ok ? "" : $" — {impact}")}");
        }

        private static void AppendPatch(StringBuilder sb, string name, MethodInfo? target, string feature)
        {
            if (target != null)
            {
                sb.AppendLine($"  [OK] {name} -> {DescribeMethod(target)} ({feature})");
                return;
            }

            sb.AppendLine($"  [MISSING] {name} ({feature})");
            sb.AppendLine($"           {GetPatchHint(name)}");
        }

        private static void AppendType(StringBuilder sb, string name, Type? type)
        {
            sb.AppendLine(type != null
                ? $"  [OK] {name} ({type.Assembly.GetName().Version})"
                : $"  [MISSING] {name}");
        }

        private static void AppendMember(StringBuilder sb, string name, MemberInfo? member)
        {
            sb.AppendLine(member != null
                ? $"  [OK] {name}"
                : $"  [MISSING] {name}");
        }

        private static string DescribeAssembly(Assembly assembly)
        {
            var name = assembly.GetName();
            var parts = new List<string> { $"v{name.Version}" };

            var fileVersion = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
            if (!string.IsNullOrEmpty(fileVersion) && fileVersion != name.Version?.ToString())
                parts.Add($"file={fileVersion}");

            var infoVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            if (!string.IsNullOrEmpty(infoVersion))
                parts.Add($"info={infoVersion}");

            if (!string.IsNullOrEmpty(assembly.Location))
                parts.Add(Path.GetFileName(assembly.Location));

            return $"{name.Name} ({string.Join(", ", parts)})";
        }

        private static string? DescribeLoadedAssembly(string assemblyName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name == assemblyName)
                    return DescribeAssembly(assembly);
            }

            return null;
        }

        private static string DescribeMethod(MethodInfo method)
        {
            var parameters = string.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name));
            return $"{method.DeclaringType?.Name}.{method.Name}({parameters})";
        }
    }
}
