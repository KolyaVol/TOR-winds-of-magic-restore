using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Settings;

namespace WindsOfMagicRestore.Utilities
{
    internal static class AugmentBuffTracker
    {
        private static readonly Type? StatusEffectComponentType =
            Type.GetType("TOR_Core.BattleMechanics.StatusEffect.StatusEffectComponent, TOR_Core");

        private static readonly Type? StatusEffectType =
            Type.GetType("TOR_Core.BattleMechanics.StatusEffect.StatusEffect, TOR_Core");

        private static readonly FieldInfo? CurrentEffectsField =
            StatusEffectComponentType?.GetField("_currentEffects", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly Func<object, object?>? ApplierAgentAccessor =
            BuildMemberAccessor("ApplierAgent");

        private static readonly Func<object, object?>? CastIdAccessor =
            BuildMemberAccessor("CastId");

        private static readonly Func<object, object?>? CurrentDurationAccessor =
            BuildMemberAccessor("CurrentDuration");

        private static Func<object, object?>? BuildMemberAccessor(string memberName)
        {
            if (StatusEffectType == null)
                return null;

            var property = StatusEffectType.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public);
            if (property != null)
                return instance => property.GetValue(instance);

            var field = StatusEffectType.GetField(memberName, BindingFlags.Instance | BindingFlags.Public);
            if (field != null)
                return instance => field.GetValue(instance);

            return null;
        }

        private static readonly MethodInfo? GetStatusEffectComponent = typeof(Agent)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(m => m.Name == "GetComponent" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 1);

        private static readonly HashSet<int> PlayerAugmentCastIds = new();
        private static readonly HashSet<int> PlayerHealCastIds = new();

        public static bool IsStatusEffectComponentTypeResolved => StatusEffectComponentType != null;
        public static bool IsStatusEffectTypeResolved => StatusEffectType != null;
        public static bool IsCurrentEffectsFieldResolved => CurrentEffectsField != null;
        public static bool IsApplierAgentAccessorResolved => ApplierAgentAccessor != null;
        public static bool IsCastIdAccessorResolved => CastIdAccessor != null;
        public static bool IsCurrentDurationAccessorResolved => CurrentDurationAccessor != null;
        public static bool IsGetStatusEffectComponentResolved => GetStatusEffectComponent != null;

        public static void Reset()
        {
            PlayerAugmentCastIds.Clear();
            PlayerHealCastIds.Clear();
        }

        public static void RegisterFromSpellSession(Agent caster, object abilityTemplate, int castId)
        {
            if (castId < 0 || caster != Agent.Main || abilityTemplate == null)
                return;

            var effectType = SpellEffectTypeHelper.GetAbilityEffectType(abilityTemplate);
            if (effectType == SpellEffectTypeHelper.Augment)
                PlayerAugmentCastIds.Add(castId);
            else if (effectType == SpellEffectTypeHelper.Heal)
                PlayerHealCastIds.Add(castId);
        }

        public static bool HasActivePlayerBuffForAugmentKills(Agent agent)
        {
            if (agent == null || Agent.Main == null)
                return false;

            var countHealAsAugment = WindsOfMagicRestoreSettings.Instance?.CountHealSpellsAsAugment ?? false;
            if (PlayerAugmentCastIds.Count == 0 && (!countHealAsAugment || PlayerHealCastIds.Count == 0))
                return false;

            if (StatusEffectComponentType == null || CurrentEffectsField == null)
                return false;

            if (GetStatusEffectComponent == null)
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
                if (ApplierAgentAccessor?.Invoke(effect) as Agent != Agent.Main)
                    continue;

                var castId = (int)(CastIdAccessor?.Invoke(effect) ?? -1);
                if (castId < 0)
                    continue;

                var duration = (float)(CurrentDurationAccessor?.Invoke(effect) ?? 0f);
                if (duration <= 0f)
                    continue;

                if (PlayerAugmentCastIds.Contains(castId))
                    return true;

                if (countHealAsAugment && PlayerHealCastIds.Contains(castId))
                    return true;
            }

            return false;
        }
    }
}
