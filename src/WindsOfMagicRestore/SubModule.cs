using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Behaviors;
using WindsOfMagicRestore.Patches;
using WindsOfMagicRestore.Settings;

namespace WindsOfMagicRestore
{
    public class SubModule : MBSubModuleBase
    {
        private const string HarmonyId = "com.windsofmagic.restore";
        private static bool _patchesApplied;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
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
            var patchedAny = false;

            patchedAny |= TryPatch(
                harmony,
                FinalizeSessionPatch.TargetMethod(),
                typeof(FinalizeSessionPatch),
                nameof(FinalizeSessionPatch.Postfix));

            patchedAny |= TryPatch(
                harmony,
                CreateSpellSessionPatch.TargetMethod(),
                typeof(CreateSpellSessionPatch),
                nameof(CreateSpellSessionPatch.Postfix));

            if (!patchedAny)
                Debug.Print("[WindsOfMagicRestore] Could not apply TOR_Core patches; heal and augment rewards may be disabled.");

            _patchesApplied = patchedAny;
        }

        private static bool TryPatch(Harmony harmony, MethodInfo? target, System.Type patchType, string postfixName)
        {
            if (target == null)
                return false;

            harmony.Patch(target, postfix: new HarmonyMethod(patchType, postfixName));
            return true;
        }
    }
}
