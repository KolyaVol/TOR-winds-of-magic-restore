using System.Reflection;
using TaleWorlds.MountAndBlade;
using WindsOfMagicRestore.Utilities;

namespace WindsOfMagicRestore.Patches
{
    public static class BookSpellKillPatch
    {
        public static MethodInfo? TargetMethod() => TorTypes.BookSpellKillMethod();

        public static void Postfix(int castId, Agent victim, object __instance)
        {
            ModGuard.Run("BookSpellKill", () =>
            {
                if (castId < 0 || victim == null || __instance == null)
                    return;

                var caster = SpellSessionResolver.ResolveCaster(__instance, castId);
                KillRewardService.TryGrantForSpellKill(victim, caster);
            });
        }
    }
}
