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

            var target = FinalizeSessionPatch.TargetMethod();
            if (target == null)
            {
                Debug.Print("[WindsOfMagicRestore] Could not find TOR_Core FinalizeSession; heal rewards disabled.");
                return;
            }

            var harmony = new Harmony(HarmonyId);
            harmony.Patch(
                target,
                postfix: new HarmonyMethod(typeof(FinalizeSessionPatch), nameof(FinalizeSessionPatch.Postfix)));

            _patchesApplied = true;
        }
    }
}
