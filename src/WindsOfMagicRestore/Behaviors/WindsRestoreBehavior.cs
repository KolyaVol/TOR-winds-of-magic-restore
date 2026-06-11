using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Settings;

namespace WindsOfMagicRestore.Behaviors
{
    public class WindsRestoreBehavior : MissionLogic
    {
        private static readonly MethodInfo? AddWindsOfMagic = Type
            .GetType("TOR_Core.Extensions.HeroExtensions, TOR_Core")
            ?.GetMethod(
                "AddWindsOfMagic",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(Hero), typeof(float) },
                null);

        private static bool _torApiMissingLogged;

        public override void OnAgentRemoved(
            Agent affectedAgent,
            Agent affectorAgent,
            AgentState agentState,
            KillingBlow blow)
        {
            if (agentState != AgentState.Killed)
                return;

            if (affectorAgent != Agent.Main)
                return;

            if (Hero.MainHero == null)
                return;

            if (!affectedAgent.IsEnemyOf(affectorAgent))
                return;

            if (AddWindsOfMagic == null)
            {
                if (!_torApiMissingLogged)
                {
                    _torApiMissingLogged = true;
                    Debug.Print("[WindsOfMagicRestore] TOR_Core.Extensions.HeroExtensions.AddWindsOfMagic not found. Is TOR_Core loaded?");
                }
                return;
            }

            var tier = (affectedAgent.Character as CharacterObject)?.Tier ?? 1;
            if (tier < 1)
                tier = 1;
            else if (tier > 6)
                tier = 6;

            var amount = WindsOfMagicRestoreSettings.Instance?.GetWindsForTier(tier) ?? 1f;
            if (amount <= 0f)
                return;

            AddWindsOfMagic.Invoke(null, new object[] { Hero.MainHero, amount });
        }
    }
}
