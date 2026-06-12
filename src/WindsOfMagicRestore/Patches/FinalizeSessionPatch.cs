using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Settings;
using WindsOfMagicRestore.Utilities;

namespace WindsOfMagicRestore.Patches
{
    public static class FinalizeSessionPatch
    {
        private static readonly Type? SessionType =
            Type.GetType("TOR_Core.AbilitySystem.SpellCasting.SpellCastSession, TOR_Core");

        private static readonly PropertyInfo? TotalHealingDone =
            SessionType?.GetProperty("TotalHealingDone");

        private static readonly PropertyInfo? Caster =
            SessionType?.GetProperty("Caster");

        public static MethodInfo? TargetMethod()
        {
            var logicType = Type.GetType("TOR_Core.AbilitySystem.AbilityManagerMissionLogic, TOR_Core");
            if (logicType == null || SessionType == null)
                return null;

            return AccessTools.Method(logicType, "FinalizeSession", new[] { SessionType });
        }

        public static void Postfix(object session)
        {
            if (session == null || Hero.MainHero == null || Agent.Main == null)
                return;

            var healingDone = (int)(TotalHealingDone?.GetValue(session) ?? 0);
            if (healingDone <= 0)
                return;

            var caster = Caster?.GetValue(session) as Agent;
            if (caster != Agent.Main)
                return;

            var settings = WindsOfMagicRestoreSettings.Instance;
            if (settings == null)
                return;

            var amount = settings.GetWindsForHealing(healingDone);
            TorWindsApi.AddWinds(amount);
        }
    }
}
