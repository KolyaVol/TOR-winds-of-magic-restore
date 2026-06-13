namespace WindsOfMagicRestore.Settings
{
    public sealed partial class WindsOfMagicRestoreSettings
    {
        private const float DamageHpPerWindBlock = 100f;

        public float GetWindsForTier(int tier)
        {
            if (!EnableKillRewards)
                return 0f;

            if (!UsePerTierKillRewards)
                return WindsPerKillAllTiers;

            return GetPerTierKillReward(
                tier,
                WindsPerKillTier1,
                WindsPerKillTier2,
                WindsPerKillTier3,
                WindsPerKillTier4,
                WindsPerKillTier5,
                WindsPerKillTier6,
                WindsPerKillTier7,
                WindsPerKillTier8,
                WindsPerKillTier9);
        }

        public float GetWindsForAugmentKillTier(int tier)
        {
            if (!EnableAugmentKillRewards)
                return 0f;

            if (!UsePerTierAugmentKillRewards)
                return WindsPerAugmentKillAllTiers;

            return GetPerTierKillReward(
                tier,
                WindsPerAugmentKillTier1,
                WindsPerAugmentKillTier2,
                WindsPerAugmentKillTier3,
                WindsPerAugmentKillTier4,
                WindsPerAugmentKillTier5,
                WindsPerAugmentKillTier6,
                WindsPerAugmentKillTier7,
                WindsPerAugmentKillTier8,
                WindsPerAugmentKillTier9);
        }

        public float GetWindsForHealing(int totalHpHealed)
        {
            if (totalHpHealed <= 0 || HealHpPerWind <= 0f || WindsPerHealBlock <= 0f)
                return 0f;

            return totalHpHealed / HealHpPerWind * WindsPerHealBlock;
        }

        public float GetWindsForMeleeDamage(float damageDealt)
        {
            return GetWindsForDamage(damageDealt, DamageHpPerWindBlock, WindsPerMeleeDamageBlock);
        }

        public float GetWindsForSpellDamage(int damageDealt)
        {
            return GetWindsForDamage(damageDealt, DamageHpPerWindBlock, WindsPerSpellDamageBlock);
        }

        public float GetWindsForFriendlyMeleeDamage(float damageDealt)
        {
            return GetWindsForDamage(damageDealt, DamageHpPerWindBlock, WindsPerFriendlyMeleeDamageBlock);
        }

        public float GetWindsForFriendlySpellDamage(int damageDealt)
        {
            return GetWindsForDamage(damageDealt, DamageHpPerWindBlock, WindsPerFriendlySpellDamageBlock);
        }

        public float GetCompanionWindsForTier(int tier)
        {
            if (!EnableCompanionKillRewards)
                return 0f;

            if (!CompanionUsePerTierKillRewards)
                return CompanionWindsPerKillAllTiers;

            return GetPerTierKillReward(
                tier,
                CompanionWindsPerKillTier1,
                CompanionWindsPerKillTier2,
                CompanionWindsPerKillTier3,
                CompanionWindsPerKillTier4,
                CompanionWindsPerKillTier5,
                CompanionWindsPerKillTier6,
                CompanionWindsPerKillTier7,
                CompanionWindsPerKillTier8,
                CompanionWindsPerKillTier9);
        }

        public float GetCompanionWindsForHealing(int totalHpHealed)
        {
            if (!EnableCompanionHealRewards || totalHpHealed <= 0 || CompanionHealHpPerWind <= 0f || CompanionWindsPerHealBlock <= 0f)
                return 0f;

            return totalHpHealed / CompanionHealHpPerWind * CompanionWindsPerHealBlock;
        }

        public float GetCompanionWindsForMeleeDamage(float damageDealt)
        {
            return GetWindsForDamage(damageDealt, DamageHpPerWindBlock, CompanionWindsPerMeleeDamageBlock);
        }

        public float GetCompanionWindsForSpellDamage(int damageDealt)
        {
            return GetWindsForDamage(damageDealt, DamageHpPerWindBlock, CompanionWindsPerSpellDamageBlock);
        }

        public float GetCompanionWindsForFriendlyMeleeDamage(float damageDealt)
        {
            return GetWindsForDamage(damageDealt, DamageHpPerWindBlock, CompanionWindsPerFriendlyMeleeDamageBlock);
        }

        public float GetCompanionWindsForFriendlySpellDamage(int damageDealt)
        {
            return GetWindsForDamage(damageDealt, DamageHpPerWindBlock, CompanionWindsPerFriendlySpellDamageBlock);
        }

        public CompanionWindsRecipientMode GetCompanionKillRestoreMode()
        {
            return CompanionWindsRecipientOption.GetMode(CompanionKillRestoreTarget);
        }

        public CompanionWindsRecipientMode GetCompanionHealRestoreMode()
        {
            return CompanionWindsRecipientOption.GetMode(CompanionHealRestoreTarget);
        }

        public CompanionWindsRecipientMode GetCompanionDamageRestoreMode()
        {
            return CompanionWindsRecipientOption.GetMode(CompanionDamageRestoreTarget);
        }

        private static float GetPerTierKillReward(
            int tier,
            float tier1,
            float tier2,
            float tier3,
            float tier4,
            float tier5,
            float tier6,
            float tier7,
            float tier8,
            float tier9)
        {
            return tier switch
            {
                1 => tier1,
                2 => tier2,
                3 => tier3,
                4 => tier4,
                5 => tier5,
                6 => tier6,
                7 => tier7,
                8 => tier8,
                9 => tier9,
                _ => 0f,
            };
        }

        private static float GetWindsForDamage(float damageDealt, float hpPerWind, float windsPerBlock)
        {
            if (damageDealt <= 0f || hpPerWind <= 0f || windsPerBlock <= 0f)
                return 0f;

            return damageDealt / hpPerWind * windsPerBlock;
        }
    }
}
