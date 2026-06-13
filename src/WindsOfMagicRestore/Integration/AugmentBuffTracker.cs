using System.Collections;
using System.Collections.Generic;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Infrastructure;
using WindsOfMagicRestore.Settings;

namespace WindsOfMagicRestore.Integration
{
    internal static class AugmentBuffTracker
    {
        private static readonly HashSet<int> PlayerAugmentCastIds = new();
        private static readonly HashSet<int> PlayerHealCastIds = new();

        public static bool IsTrackingAvailable => StatusEffectReflection.HasDurationTracking;

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
            var component = StatusEffectReflection.GetComponent(agent);
            if (component == null)
                return false;

            if (StatusEffectReflection.GetCurrentEffects(component) is not IDictionary effects)
                return false;

            foreach (DictionaryEntry entry in effects)
            {
                if (entry.Key == null)
                    continue;

                var effect = entry.Key;
                if (StatusEffectReflection.ApplierAgent!(effect) as Agent != Agent.Main)
                    continue;

                var castId = (int)(StatusEffectReflection.CastId!(effect) ?? -1);
                if (castId < 0)
                    continue;

                var duration = (float)(StatusEffectReflection.CurrentDuration!(effect) ?? 0f);
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
