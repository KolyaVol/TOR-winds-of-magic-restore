using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace WindsOfMagicRestore.Integration
{
    internal static class TorTypes
    {
        private const string TorAssembly = "TOR_Core";

        public static readonly Type? AbilityManagerMissionLogic = ResolveTor("TOR_Core.AbilitySystem.AbilityManagerMissionLogic");
        public static readonly Type? AbilityTemplate = ResolveTor("TOR_Core.AbilitySystem.AbilityTemplate");
        public static readonly Type? SpellCastSession = ResolveTor("TOR_Core.AbilitySystem.SpellCasting.SpellCastSession");
        public static readonly Type? StatusEffectComponent = ResolveTor("TOR_Core.BattleMechanics.StatusEffect.StatusEffectComponent");
        public static readonly Type? StatusEffect = ResolveTor("TOR_Core.BattleMechanics.StatusEffect.StatusEffect");
        public static readonly Type? TorAgentApplyDamageModel = ResolveTor("TOR_Core.Models.TORAgentApplyDamageModel");
        public static readonly Type? AttackInformation = Type.GetType("TaleWorlds.MountAndBlade.AttackInformation, TaleWorlds.MountAndBlade");

        public static readonly MethodInfo? AddWindsOfMagic = ResolveStaticMethod(
            "TOR_Core.Extensions.HeroExtensions",
            "AddWindsOfMagic",
            typeof(Hero),
            typeof(float));

        public static readonly MethodInfo? AddCustomResource = ResolveStaticMethod(
            "TOR_Core.Extensions.HeroExtensions",
            "AddCustomResource",
            typeof(Hero),
            typeof(string),
            typeof(float));

        public static readonly MethodInfo? BelongsToMainParty = ResolveStaticMethod(
            "TOR_Core.Extensions.AgentExtensions",
            "BelongsToMainParty",
            typeof(Agent));

        public static readonly PropertyInfo? SessionTotalHealingDone = SpellCastSession?.GetProperty("TotalHealingDone");
        public static readonly PropertyInfo? SessionCaster = SpellCastSession?.GetProperty("Caster");
        public static readonly PropertyInfo? SessionCasterHero = SpellCastSession?.GetProperty("CasterHero");
        public static readonly PropertyInfo? SessionCastId = SpellCastSession?.GetProperty("CastID");

        public static readonly FieldInfo? ActiveSpellSessionsField =
            AbilityManagerMissionLogic?.GetField("_activeSpellSessions", BindingFlags.Instance | BindingFlags.NonPublic);

        public static readonly FieldInfo? PendingCollectSessionsField =
            AbilityManagerMissionLogic?.GetField("_pendingCollectSessions", BindingFlags.Instance | BindingFlags.NonPublic);

        public static readonly FieldInfo? StatusEffectCurrentEffectsField =
            StatusEffectComponent?.GetField("_currentEffects", BindingFlags.Instance | BindingFlags.NonPublic);

        public static MethodInfo? FinalizeSessionMethod()
        {
            if (AbilityManagerMissionLogic == null || SpellCastSession == null)
                return null;

            return AccessTools.Method(AbilityManagerMissionLogic, "FinalizeSession", new[] { SpellCastSession });
        }

        public static MethodInfo? CreateSpellSessionMethod()
        {
            if (AbilityManagerMissionLogic == null || AbilityTemplate == null)
                return null;

            return AccessTools.Method(AbilityManagerMissionLogic, "CreateSpellSession", new[] { typeof(Agent), AbilityTemplate });
        }

        public static MethodInfo? BookSpellKillMethod()
        {
            return AbilityManagerMissionLogic?.GetMethod(
                "BookSpellKill",
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(int), typeof(Agent) },
                null);
        }

        public static MethodInfo? BookSpellDamageMethod()
        {
            if (AbilityManagerMissionLogic == null)
                return null;

            return AccessTools.Method(AbilityManagerMissionLogic, "BookSpellDamage");
        }

        public static MethodInfo? ApplyGeneralDamageModifiersMethod()
        {
            if (TorAgentApplyDamageModel == null)
                return null;

            return AccessTools.DeclaredMethod(TorAgentApplyDamageModel, "ApplyGeneralDamageModifiers");
        }

        private static Type? ResolveTor(string typeName) => Type.GetType($"{typeName}, {TorAssembly}");

        private static MethodInfo? ResolveStaticMethod(string typeName, string methodName, params Type[] parameterTypes)
        {
            return ResolveTor(typeName)?.GetMethod(
                methodName,
                BindingFlags.Public | BindingFlags.Static,
                null,
                parameterTypes,
                null);
        }
    }
}
