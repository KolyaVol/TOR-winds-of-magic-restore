using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using TaleWorlds.MountAndBlade;

namespace WindsOfMagicRestore.Utilities
{
    internal static class StatusEffectHelper
    {
        private static readonly Func<object, object?>? ApplierAgentAccessor =
            BuildMemberAccessor("ApplierAgent");

        private static readonly Func<object, object?>? CastIdAccessor =
            BuildMemberAccessor("CastId");

        private static readonly MethodInfo? GetStatusEffectComponent = typeof(Agent)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(m => m.Name == "GetComponent" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 1);

        public static bool IsAvailable =>
            TorTypes.StatusEffectComponent != null
            && TorTypes.StatusEffect != null
            && TorTypes.StatusEffectCurrentEffectsField != null
            && ApplierAgentAccessor != null
            && CastIdAccessor != null
            && GetStatusEffectComponent != null;

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
            var component = GetStatusEffectComponent!
                .MakeGenericMethod(TorTypes.StatusEffectComponent!)
                .Invoke(victim, null);
            if (component == null)
                return null;

            if (TorTypes.StatusEffectCurrentEffectsField!.GetValue(component) is not IDictionary effects)
                return null;

            Agent? fallbackApplier = null;

            foreach (DictionaryEntry entry in effects)
            {
                if (entry.Key == null)
                    continue;

                var effect = entry.Key;
                var applier = KillCreditHelper.NormalizeAgent(ApplierAgentAccessor!(effect) as Agent);
                if (applier == null)
                    continue;

                if (castId >= 0)
                {
                    var effectCastId = (int)(CastIdAccessor!(effect) ?? -1);
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

        private static Func<object, object?>? BuildMemberAccessor(string memberName)
        {
            if (TorTypes.StatusEffect == null)
                return null;

            var property = TorTypes.StatusEffect.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public);
            if (property != null)
                return instance => property.GetValue(instance);

            var field = TorTypes.StatusEffect.GetField(memberName, BindingFlags.Instance | BindingFlags.Public);
            if (field != null)
                return instance => field.GetValue(instance);

            return null;
        }
    }
}
