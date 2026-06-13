using System;
using System.Linq;
using System.Reflection;
using TaleWorlds.MountAndBlade;

namespace WindsOfMagicRestore.Integration
{
    internal static class StatusEffectReflection
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

        public static bool IsAvailable =>
            TorTypes.StatusEffectComponent != null
            && TorTypes.StatusEffect != null
            && TorTypes.StatusEffectCurrentEffectsField != null
            && ApplierAgentAccessor != null
            && CastIdAccessor != null
            && GetStatusEffectComponent != null;

        public static bool HasDurationTracking =>
            IsAvailable && CurrentDurationAccessor != null;

        public static Func<object, object?>? ApplierAgent => ApplierAgentAccessor;

        public static Func<object, object?>? CastId => CastIdAccessor;

        public static Func<object, object?>? CurrentDuration => CurrentDurationAccessor;

        public static object? GetComponent(Agent agent)
        {
            if (!IsAvailable || TorTypes.StatusEffectComponent == null)
                return null;

            return GetStatusEffectComponent!
                .MakeGenericMethod(TorTypes.StatusEffectComponent)
                .Invoke(agent, null);
        }

        public static System.Collections.IDictionary? GetCurrentEffects(object component)
        {
            if (component == null || TorTypes.StatusEffectCurrentEffectsField == null)
                return null;

            return TorTypes.StatusEffectCurrentEffectsField.GetValue(component) as System.Collections.IDictionary;
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
