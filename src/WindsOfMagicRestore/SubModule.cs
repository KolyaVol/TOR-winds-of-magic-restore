using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Behaviors;
using WindsOfMagicRestore.Infrastructure;
using WindsOfMagicRestore.Patches;
using WindsOfMagicRestore.Settings;

namespace WindsOfMagicRestore
{
    public class SubModule : MBSubModuleBase
    {
        private const string HarmonyId = "com.windsofmagic.restore";
        private static bool _patchesApplied;

        private static readonly (string Name, Func<MethodInfo?> Target, Type PatchType, string Postfix)[] Patches =
        {
            ("FinalizeSession", FinalizeSessionPatch.TargetMethod, typeof(FinalizeSessionPatch), nameof(FinalizeSessionPatch.Postfix)),
            ("CreateSpellSession", CreateSpellSessionPatch.TargetMethod, typeof(CreateSpellSessionPatch), nameof(CreateSpellSessionPatch.Postfix)),
            ("BookSpellKill", BookSpellKillPatch.TargetMethod, typeof(BookSpellKillPatch), nameof(BookSpellKillPatch.Postfix)),
            ("BookSpellDamage", BookSpellDamagePatch.TargetMethod, typeof(BookSpellDamagePatch), nameof(BookSpellDamagePatch.Postfix)),
            ("ApplyGeneralDamageModifiers", ApplyGeneralDamageModifiersPatch.TargetMethod, typeof(ApplyGeneralDamageModifiersPatch), nameof(ApplyGeneralDamageModifiersPatch.Postfix)),
        };

        protected override void OnSubModuleLoad()
        {
            ModTrace.Mark("OnSubModuleLoad:enter");
            base.OnSubModuleLoad();
            ModTrace.Mark("OnSubModuleLoad:exit");
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            ModTrace.Mark("OnBeforeInitialModuleScreenSetAsRoot:enter");
            base.OnBeforeInitialModuleScreenSetAsRoot();
            ModGuard.Run("OnBeforeInitialModuleScreenSetAsRoot", () =>
            {
                WindsOfMagicRestoreSettings.Initialize();
                ModDiagnostics.LogStartupReport();
                TryApplyPatches();
            });
            ModTrace.Mark("OnBeforeInitialModuleScreenSetAsRoot:exit");
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

            ModTrace.Mark("patches:start");
            var harmony = new Harmony(HarmonyId);
            var applied = 0;

            foreach (var patch in Patches)
            {
                if (TryPatch(harmony, patch.Name, patch.Target(), patch.PatchType, patch.Postfix))
                    applied++;
            }

            ModDiagnostics.LogPatchResults("harmony", applied, Patches.Length);
            _patchesApplied = applied > 0;
            ModTrace.Mark($"patches:done {applied}/{Patches.Length}");
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
