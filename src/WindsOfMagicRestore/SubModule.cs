using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Behaviors;
using WindsOfMagicRestore.Patches;
using WindsOfMagicRestore.Settings;
using WindsOfMagicRestore.Utilities;

namespace WindsOfMagicRestore
{
    public class SubModule : MBSubModuleBase
    {
        private const string HarmonyId = "com.windsofmagic.restore";
        private const int PatchCount = 5;
        private static bool _patchesApplied;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            ModDiagnostics.LogStartupReport();
            WindsOfMagicRestoreSettings.Initialize();
            TryApplyPatches();
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            WindsOfMagicRestoreSettings.Initialize();
            TryApplyPatches();
        }

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);

            if (Campaign.Current == null)
                return;

            mission.AddMissionBehavior(new WindsRestoreBehavior());
        }

        private static void TryApplyPatches()
        {
            if (_patchesApplied)
                return;

            var harmony = new Harmony(HarmonyId);
            var applied = 0;

            applied += TryPatch(harmony, "FinalizeSession", FinalizeSessionPatch.TargetMethod(), typeof(FinalizeSessionPatch), nameof(FinalizeSessionPatch.Postfix)) ? 1 : 0;
            applied += TryPatch(harmony, "CreateSpellSession", CreateSpellSessionPatch.TargetMethod(), typeof(CreateSpellSessionPatch), nameof(CreateSpellSessionPatch.Postfix)) ? 1 : 0;
            applied += TryPatch(harmony, "BookSpellKill", BookSpellKillPatch.TargetMethod(), typeof(BookSpellKillPatch), nameof(BookSpellKillPatch.Postfix)) ? 1 : 0;
            applied += TryPatch(harmony, "BookSpellDamage", BookSpellDamagePatch.TargetMethod(), typeof(BookSpellDamagePatch), nameof(BookSpellDamagePatch.Postfix)) ? 1 : 0;
            applied += TryPatch(harmony, "ApplyGeneralDamageModifiers", ApplyGeneralDamageModifiersPatch.TargetMethod(), typeof(ApplyGeneralDamageModifiersPatch), nameof(ApplyGeneralDamageModifiersPatch.Postfix)) ? 1 : 0;

            ModDiagnostics.LogPatchResults(applied, PatchCount);
            _patchesApplied = applied > 0;
        }

        private static bool TryPatch(Harmony harmony, string patchName, MethodInfo? target, Type patchType, string postfixName)
        {
            if (target == null)
            {
                ModLog.OnceWarn(
                    $"patch:missing:{patchName}",
                    $"Harmony target not found: {patchName}. {ModDiagnostics.GetPatchHint(patchName)}");
                return false;
            }

            try
            {
                harmony.Patch(target, postfix: new HarmonyMethod(patchType, postfixName));
                ModLog.Info($"Patched {target.DeclaringType?.FullName}.{target.Name} <- {patchName}");
                return true;
            }
            catch (Exception ex)
            {
                ModLog.Error(
                    $"Harmony patch failed: {patchName} on {target.DeclaringType?.FullName}.{target.Name} — {ModLog.FormatException(ex)}");
                return false;
            }
        }
    }
}
