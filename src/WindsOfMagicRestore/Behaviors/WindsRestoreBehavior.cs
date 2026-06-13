using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Domain;
using WindsOfMagicRestore.Infrastructure;
using WindsOfMagicRestore.Integration;
using WindsOfMagicRestore.Settings;

namespace WindsOfMagicRestore.Behaviors
{
    public class WindsRestoreBehavior : MissionLogic
    {
        public override void AfterStart()
        {
            AugmentBuffTracker.Reset();
            KillRewardTracker.Reset();
            SpellCastRegistry.Reset();
            WindsRestoreMessages.Reset();
            ModDiagnostics.TryShowBattleWarning();
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
            WindsRestoreMessages.Tick(Mission.CurrentTime);

            if (Hero.MainHero == null || Agent.Main == null || !Agent.Main.IsActive())
                return;

            if (Mission.IsFriendlyMission || Mission.IsMissionEnding)
                return;

            var rate = WindsOfMagicRestoreSettings.Instance?.WindsPerSecond ?? 0f;
            if (rate <= 0f)
                return;

            TorWindsApi.AddWinds(rate * dt);
        }

        protected override void OnEndMission()
        {
            WindsRestoreMessages.FlushRemaining(Mission.CurrentTime);
            WindsRestoreMessages.Reset();
        }
    }
}
