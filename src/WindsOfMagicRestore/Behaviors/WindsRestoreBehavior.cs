using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Settings;
using WindsOfMagicRestore.Utilities;

namespace WindsOfMagicRestore.Behaviors
{
    public class WindsRestoreBehavior : MissionLogic
    {
        public override void AfterStart()
        {
            AugmentBuffTracker.Reset();
            KillRewardTracker.Reset();
        }

        public override void OnEarlyAgentRemoved(
            Agent affectedAgent,
            Agent affectorAgent,
            AgentState agentState,
            KillingBlow blow)
        {
            KillRewardService.TryGrantForAgentRemoval(
                affectedAgent,
                affectorAgent,
                agentState,
                blow,
                Mission);
        }

        public override void OnMissionTick(float dt)
        {
            if (Hero.MainHero == null || Agent.Main == null || !Agent.Main.IsActive())
                return;

            if (Mission.IsFriendlyMission || Mission.IsMissionEnding)
                return;

            var rate = WindsOfMagicRestoreSettings.Instance?.WindsPerSecond ?? 0f;
            if (rate <= 0f)
                return;

            TorWindsApi.AddWinds(rate * dt);
        }
    }
}
