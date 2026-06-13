using System.Collections;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace WindsOfMagicRestore.Utilities
{
    internal static class SpellSessionResolver
    {
        public static Agent? ResolveCaster(object logic, int castId, Agent? victim = null)
        {
            return ModGuard.Run("ResolveCaster", () => ResolveCasterCore(logic, castId, victim));
        }

        private static Agent? ResolveCasterCore(object logic, int castId, Agent? victim)
        {
            var session = ResolveSession(logic, castId);
            if (session != null)
            {
                var caster = KillCreditHelper.NormalizeAgent(TorTypes.SessionCaster?.GetValue(session) as Agent);
                if (caster == null)
                {
                    var casterHero = TorTypes.SessionCasterHero?.GetValue(session) as Hero;
                    caster = CompanionHelper.ResolveAgentForHero(casterHero);
                }

                if (caster != null)
                {
                    SpellCastRegistry.Register(castId, caster);
                    return caster;
                }
            }

            var registeredCaster = SpellCastRegistry.ResolveCaster(castId);
            if (registeredCaster != null)
                return registeredCaster;

            if (victim != null)
                return StatusEffectHelper.ResolveApplierAgent(victim, castId);

            return null;
        }

        private static object? ResolveSession(object logic, int castId)
        {
            if (TorTypes.ActiveSpellSessionsField?.GetValue(logic) is IDictionary activeSessions
                && activeSessions.Contains(castId))
            {
                return activeSessions[castId];
            }

            if (TorTypes.PendingCollectSessionsField?.GetValue(logic) is not IEnumerable pendingSessions)
                return null;

            foreach (var pendingSession in pendingSessions)
            {
                if (pendingSession == null)
                    continue;

                var pendingCastId = (int)(TorTypes.SessionCastId?.GetValue(pendingSession) ?? -1);
                if (pendingCastId == castId)
                    return pendingSession;
            }

            return null;
        }
    }
}
