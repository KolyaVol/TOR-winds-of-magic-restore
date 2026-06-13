using System.Collections;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Battle;
using WindsOfMagicRestore.Infrastructure;

namespace WindsOfMagicRestore.Integration
{
    internal static class StatusEffectHelper
    {
        public static bool IsAvailable => StatusEffectReflection.IsAvailable;

        public static Agent? ResolveApplierAgent(Agent victim, int castId = -1)
        {
            if (victim == null || !IsAvailable)
                return null;

            return ModGuard.Run(
                "ResolveApplierAgent",
                () => ResolveApplierAgentCore(victim, castId));
        }

        private static Agent? ResolveApplierAgentCore(Agent victim, int castId)
        {
            var component = StatusEffectReflection.GetComponent(victim);
            if (component == null)
                return null;

            if (StatusEffectReflection.GetCurrentEffects(component) is not IDictionary effects)
                return null;

            Agent? fallbackApplier = null;

            foreach (DictionaryEntry entry in effects)
            {
                if (entry.Key == null)
                    continue;

                var effect = entry.Key;
                var applier = KillCreditHelper.NormalizeAgent(StatusEffectReflection.ApplierAgent!(effect) as Agent);
                if (applier == null)
                    continue;

                if (castId >= 0)
                {
                    var effectCastId = (int)(StatusEffectReflection.CastId!(effect) ?? -1);
                    if (effectCastId == castId)
                        return applier;

                    continue;
                }

                if (KillCreditHelper.IsMainHeroAgent(applier))
                    return applier;

                if (fallbackApplier == null && AgentPartyHelper.IsMainPartyAgent(applier))
                    fallbackApplier = applier;
            }

            return fallbackApplier;
        }
    }
}
