using System.Reflection;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Utilities;

namespace WindsOfMagicRestore.Patches
{
    public static class BookSpellDamagePatch
    {
        public static MethodInfo? TargetMethod() => TorTypes.BookSpellDamageMethod();

        public static void Postfix(int castId, Agent victim, int damageDealt, object __instance)
        {
            ModGuard.Run("BookSpellDamage", () =>
            {
                if (castId < 0 || victim == null || damageDealt <= 0 || __instance == null)
                    return;

                var caster = SpellSessionResolver.ResolveCaster(__instance, castId, victim);
                DamageRewardService.TryGrantForSpellDamage(victim, caster, damageDealt);
            });
        }
    }
}
