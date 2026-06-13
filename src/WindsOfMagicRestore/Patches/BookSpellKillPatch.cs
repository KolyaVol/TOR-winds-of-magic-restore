using System.Reflection;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Domain;
using WindsOfMagicRestore.Integration;

namespace WindsOfMagicRestore.Patches
{
    public static class BookSpellKillPatch
    {
        public static MethodInfo? TargetMethod() => TorTypes.BookSpellKillMethod();

        public static void Postfix(int castId, Agent victim, object __instance)
        {
            if (castId < 0 || victim == null || __instance == null)
                return;

            var caster = SpellSessionResolver.ResolveCaster(__instance, castId, victim);
            KillRewardService.TryGrantForSpellKill(victim, caster);
        }
    }
}
