using System;
using System.Collections;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Utilities;

namespace WindsOfMagicRestore.Patches
{
    public static class BookSpellKillPatch
    {
        private static readonly Type? LogicType =
            Type.GetType("TOR_Core.AbilitySystem.AbilityManagerMissionLogic, TOR_Core");

        private static readonly Type? SessionType =
            Type.GetType("TOR_Core.AbilitySystem.SpellCasting.SpellCastSession, TOR_Core");

        private static readonly PropertyInfo? SessionCaster =
            SessionType?.GetProperty("Caster");

        private static readonly PropertyInfo? SessionCasterHero =
            SessionType?.GetProperty("CasterHero");

        private static readonly PropertyInfo? PendingSessionCastId =
            SessionType?.GetProperty("CastID");

        private static readonly FieldInfo? ActiveSessionsField =
            LogicType?.GetField("_activeSpellSessions", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo? PendingSessionsField =
            LogicType?.GetField("_pendingCollectSessions", BindingFlags.Instance | BindingFlags.NonPublic);

        public static MethodInfo? TargetMethod()
        {
            return LogicType?.GetMethod(
                "BookSpellKill",
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(int), typeof(Agent) },
                null);
        }

        public static void Postfix(int castId, Agent victim, object __instance)
        {
            if (castId < 0 || victim == null || __instance == null)
                return;

            var caster = ResolveCaster(__instance, castId);
            KillRewardService.TryGrantForSpellKill(victim, caster);
        }

        private static Agent? ResolveCaster(object logic, int castId)
        {
            var session = ResolveSession(logic, castId);
            if (session == null)
                return null;

            var caster = KillCreditHelper.NormalizeAgent(SessionCaster?.GetValue(session) as Agent);
            if (caster != null)
                return caster;

            var casterHero = SessionCasterHero?.GetValue(session) as Hero;
            if (casterHero == Hero.MainHero && Agent.Main != null)
                return Agent.Main;

            return null;
        }

        private static object? ResolveSession(object logic, int castId)
        {
            if (ActiveSessionsField?.GetValue(logic) is IDictionary activeSessions
                && activeSessions.Contains(castId))
            {
                return activeSessions[castId];
            }

            if (PendingSessionsField?.GetValue(logic) is not IEnumerable pendingSessions)
                return null;

            foreach (var pendingSession in pendingSessions)
            {
                if (pendingSession == null)
                    continue;

                var pendingCastId = (int)(PendingSessionCastId?.GetValue(pendingSession) ?? -1);
                if (pendingCastId == castId)
                    return pendingSession;
            }

            return null;
        }
    }
}
