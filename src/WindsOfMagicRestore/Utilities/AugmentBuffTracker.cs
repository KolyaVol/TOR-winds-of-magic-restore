using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.MountAndBlade;

namespace WindsOfMagicRestore.Utilities
{
    internal static class AugmentBuffTracker
    {
        private const int AugmentEffectType = 6;

        private static readonly Type? StatusEffectComponentType =
            Type.GetType("TOR_Core.BattleMechanics.StatusEffect.StatusEffectComponent, TOR_Core");

        private static readonly Type? StatusEffectType =
            Type.GetType("TOR_Core.BattleMechanics.StatusEffect.StatusEffect, TOR_Core");

        private static readonly FieldInfo? CurrentEffectsField =
            StatusEffectComponentType?.GetField("_currentEffects", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly PropertyInfo? ApplierAgentProperty =
            StatusEffectType?.GetProperty("ApplierAgent");

        private static readonly PropertyInfo? CastIdProperty =
            StatusEffectType?.GetProperty("CastId");

        private static readonly PropertyInfo? CurrentDurationProperty =
            StatusEffectType?.GetProperty("CurrentDuration");

        private static readonly MethodInfo? GetStatusEffectComponent = typeof(Agent)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(m => m.Name == "GetComponent" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 1);

        private static readonly HashSet<int> PlayerAugmentCastIds = new();

        public static void Reset()
        {
            PlayerAugmentCastIds.Clear();
        }

        public static void RegisterPlayerAugmentCast(int castId)
        {
            if (castId >= 0)
                PlayerAugmentCastIds.Add(castId);
        }

        public static bool TryRegisterFromSpellSession(Agent caster, object abilityTemplate, int castId)
        {
            if (castId < 0 || caster != Agent.Main || abilityTemplate == null)
                return false;

            var effectType = abilityTemplate.GetType().GetProperty("AbilityEffectType")?.GetValue(abilityTemplate);
            if (effectType == null || Convert.ToInt32(effectType) != AugmentEffectType)
                return false;

            RegisterPlayerAugmentCast(castId);
            return true;
        }

        public static bool HasActivePlayerAugmentBuff(Agent agent)
        {
            if (agent == null || Agent.Main == null || PlayerAugmentCastIds.Count == 0)
                return false;

            if (StatusEffectComponentType == null || CurrentEffectsField == null)
                return false;

            if (GetStatusEffectComponent == null || StatusEffectComponentType == null)
                return false;

            var component = GetStatusEffectComponent
                .MakeGenericMethod(StatusEffectComponentType)
                .Invoke(agent, null);
            if (component == null)
                return false;

            if (CurrentEffectsField.GetValue(component) is not IDictionary effects)
                return false;

            foreach (DictionaryEntry entry in effects)
            {
                if (entry.Key == null)
                    continue;

                var effect = entry.Key;
                if (ApplierAgentProperty?.GetValue(effect) as Agent != Agent.Main)
                    continue;

                var castId = (int)(CastIdProperty?.GetValue(effect) ?? -1);
                if (castId < 0 || !PlayerAugmentCastIds.Contains(castId))
                    continue;

                var duration = (float)(CurrentDurationProperty?.GetValue(effect) ?? 0f);
                if (duration > 0f)
                    return true;
            }

            return false;
        }
    }
}
