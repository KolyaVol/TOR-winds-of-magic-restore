using System;

namespace WindsOfMagicRestore.Utilities
{
    internal static class SpellEffectTypeHelper
    {
        public const int Heal = 5;
        public const int Augment = 6;

        public static int? GetAbilityEffectType(object? abilityTemplate)
        {
            if (abilityTemplate == null)
                return null;

            var effectType = abilityTemplate.GetType().GetProperty("AbilityEffectType")?.GetValue(abilityTemplate);
            return effectType == null ? null : Convert.ToInt32(effectType);
        }

        public static bool IsHealType(object? abilityTemplate) =>
            GetAbilityEffectType(abilityTemplate) == Heal;

        public static bool IsAugmentType(object? abilityTemplate) =>
            GetAbilityEffectType(abilityTemplate) == Augment;
    }
}
