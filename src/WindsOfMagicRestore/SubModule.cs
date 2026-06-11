using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Behaviors;
using WindsOfMagicRestore.Settings;

namespace WindsOfMagicRestore
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            WindsOfMagicRestoreSettings.Initialize();
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            WindsOfMagicRestoreSettings.Initialize();
        }

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);

            if (Campaign.Current == null)
                return;

            mission.AddMissionBehavior(new WindsRestoreBehavior());
        }
    }
}
