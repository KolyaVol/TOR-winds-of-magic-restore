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
        private static readonly Func<object, object?>? ApplierAgentAccessor =
            BuildMemberAccessor("ApplierAgent");

        private static readonly Func<object, object?>? CastIdAccessor =
            BuildMemberAccessor("CastId");

        private static readonly Func<object, object?>? CurrentDurationAccessor =
            BuildMemberAccessor("CurrentDuration");

        private static readonly MethodInfo? GetStatusEffectComponent = typeof(Agent)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(m => m.Name == "GetComponent" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 1);

        private static readonly HashSet<int> PlayerAugmentCastIds = new();
        private static readonly HashSet<int> PlayerHealCastIds = new();

        public static bool IsTrackingAvailable =>
            TorTypes.StatusEffectComponent != null
            && TorTypes.StatusEffect != null
            && TorTypes.StatusEffectCurrentEffectsField != null
            && ApplierAgentAccessor != null
            && CastIdAccessor != null
            && CurrentDurationAccessor != null
            && GetStatusEffectComponent != null;

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

            if (!IsTrackingAvailable)
                return false;

            return ModGuard.Run("HasActivePlayerBuff", () => HasActivePlayerBuffCore(agent, countHealAsAugment), false);
        }

        private static bool HasActivePlayerBuffCore(Agent agent, bool countHealAsAugment)
        {
            var component = GetStatusEffectComponent!
                .MakeGenericMethod(TorTypes.StatusEffectComponent!)
                .Invoke(agent, null);
            if (component == null)
                return false;

            if (TorTypes.StatusEffectCurrentEffectsField!.GetValue(component) is not IDictionary effects)
                return false;

            foreach (DictionaryEntry entry in effects)
            {
                if (entry.Key == null)
                    continue;

                var effect = entry.Key;
                if (ApplierAgentAccessor!(effect) as Agent != Agent.Main)
                    continue;

                var castId = (int)(CastIdAccessor!(effect) ?? -1);
                if (castId < 0)
                    continue;

                var duration = (float)(CurrentDurationAccessor!(effect) ?? 0f);
                if (duration <= 0f)
                    continue;

                if (PlayerAugmentCastIds.Contains(castId))
                    return true;

                if (countHealAsAugment && PlayerHealCastIds.Contains(castId))
                    return true;
            }

            return false;
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
