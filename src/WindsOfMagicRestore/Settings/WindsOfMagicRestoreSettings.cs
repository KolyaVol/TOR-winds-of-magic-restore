using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace WindsOfMagicRestore.Settings
{
    public sealed class WindsOfMagicRestoreSettings : AttributeGlobalSettings<WindsOfMagicRestoreSettings>
    {
        private float _windsPerKillAllTiers = 1f;
        private bool _usePerTierKillRewards;
        private float _windsPerKillTier1 = 1f;
        private float _windsPerKillTier2;
        private float _windsPerKillTier3;
        private float _windsPerKillTier4;
        private float _windsPerKillTier5;
        private float _windsPerKillTier6;
        private float _windsPerAugmentKillAllTiers = 1f;
        private bool _usePerTierAugmentKillRewards;
        private float _windsPerAugmentKillTier1 = 1f;
        private float _windsPerAugmentKillTier2;
        private float _windsPerAugmentKillTier3;
        private float _windsPerAugmentKillTier4;
        private float _windsPerAugmentKillTier5;
        private float _windsPerAugmentKillTier6;
        private float _windsPerHealBlock = 1f;
        private float _healHpPerWind = 100f;
        private bool _countHealSpellsAsAugment;
        private float _windsPerSecond = 0.05f;
        private float _windsPerMeleeDamageBlock;
        private float _meleeDamageHpPerWind = 100f;
        private float _windsPerFriendlyMeleeDamageBlock;
        private float _windsPerSpellDamageBlock;
        private float _spellDamageHpPerWind = 100f;
        private float _windsPerFriendlySpellDamageBlock;

        public override string Id => "WindsOfMagicRestore_v1";
        public override string DisplayName => "Winds of Magic Restore";
        public override string FolderName => "WindsOfMagicRestore";
        public override string FormatType => "json";

        public static void Initialize()
        {
            _ = Instance;
        }

        [SettingPropertyBool("Different amount per tier", IsToggle = true, Order = 0, RequireRestart = false, HintText = "Set a separate reward for each enemy tier.")]
        [SettingPropertyGroup("Kill rewards", GroupOrder = 0)]
        public bool UsePerTierKillRewards
        {
            get => _usePerTierKillRewards;
            set
            {
                if (_usePerTierKillRewards == value)
                    return;

                _usePerTierKillRewards = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(UseUniformKillReward));
            }
        }

        public bool UseUniformKillReward
        {
            get => !_usePerTierKillRewards;
            set => UsePerTierKillRewards = !value;
        }

        [SettingPropertyFloatingInteger("Winds per kill", 0f, 100f, "0.##", Order = 1, RequireRestart = false, HintText = "Same reward for every enemy tier.")]
        [SettingPropertyGroup("Kill rewards/{UseUniformKillReward}")]
        public float WindsPerKillAllTiers
        {
            get => _windsPerKillAllTiers;
            set { if (_windsPerKillAllTiers != value) { _windsPerKillAllTiers = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 1", 0f, 100f, "0.##", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards/{UsePerTierKillRewards}")]
        public float WindsPerKillTier1
        {
            get => _windsPerKillTier1;
            set { if (_windsPerKillTier1 != value) { _windsPerKillTier1 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 2", 0f, 100f, "0.##", Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards/{UsePerTierKillRewards}")]
        public float WindsPerKillTier2
        {
            get => _windsPerKillTier2;
            set { if (_windsPerKillTier2 != value) { _windsPerKillTier2 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 3", 0f, 100f, "0.##", Order = 2, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards/{UsePerTierKillRewards}")]
        public float WindsPerKillTier3
        {
            get => _windsPerKillTier3;
            set { if (_windsPerKillTier3 != value) { _windsPerKillTier3 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 4", 0f, 100f, "0.##", Order = 3, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards/{UsePerTierKillRewards}")]
        public float WindsPerKillTier4
        {
            get => _windsPerKillTier4;
            set { if (_windsPerKillTier4 != value) { _windsPerKillTier4 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 5", 0f, 100f, "0.##", Order = 4, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards/{UsePerTierKillRewards}")]
        public float WindsPerKillTier5
        {
            get => _windsPerKillTier5;
            set { if (_windsPerKillTier5 != value) { _windsPerKillTier5 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 6", 0f, 100f, "0.##", Order = 5, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards/{UsePerTierKillRewards}")]
        public float WindsPerKillTier6
        {
            get => _windsPerKillTier6;
            set { if (_windsPerKillTier6 != value) { _windsPerKillTier6 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyBool("Different amount per tier", IsToggle = true, Order = 0, RequireRestart = false, HintText = "Set a separate reward for each enemy tier.")]
        [SettingPropertyGroup("Buffed unit kills", GroupOrder = 1)]
        public bool UsePerTierAugmentKillRewards
        {
            get => _usePerTierAugmentKillRewards;
            set
            {
                if (_usePerTierAugmentKillRewards == value)
                    return;

                _usePerTierAugmentKillRewards = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(UseUniformAugmentKillReward));
            }
        }

        public bool UseUniformAugmentKillReward
        {
            get => !_usePerTierAugmentKillRewards;
            set => UsePerTierAugmentKillRewards = !value;
        }

        [SettingPropertyFloatingInteger("Winds per kill", 0f, 100f, "0.##", Order = 1, RequireRestart = false, HintText = "Winds when a troop you buffed kills an enemy. Same reward for every tier.")]
        [SettingPropertyGroup("Buffed unit kills/{UseUniformAugmentKillReward}")]
        public float WindsPerAugmentKillAllTiers
        {
            get => _windsPerAugmentKillAllTiers;
            set { if (_windsPerAugmentKillAllTiers != value) { _windsPerAugmentKillAllTiers = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 1", 0f, 100f, "0.##", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Buffed unit kills/{UsePerTierAugmentKillRewards}")]
        public float WindsPerAugmentKillTier1
        {
            get => _windsPerAugmentKillTier1;
            set { if (_windsPerAugmentKillTier1 != value) { _windsPerAugmentKillTier1 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 2", 0f, 100f, "0.##", Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("Buffed unit kills/{UsePerTierAugmentKillRewards}")]
        public float WindsPerAugmentKillTier2
        {
            get => _windsPerAugmentKillTier2;
            set { if (_windsPerAugmentKillTier2 != value) { _windsPerAugmentKillTier2 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 3", 0f, 100f, "0.##", Order = 2, RequireRestart = false)]
        [SettingPropertyGroup("Buffed unit kills/{UsePerTierAugmentKillRewards}")]
        public float WindsPerAugmentKillTier3
        {
            get => _windsPerAugmentKillTier3;
            set { if (_windsPerAugmentKillTier3 != value) { _windsPerAugmentKillTier3 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 4", 0f, 100f, "0.##", Order = 3, RequireRestart = false)]
        [SettingPropertyGroup("Buffed unit kills/{UsePerTierAugmentKillRewards}")]
        public float WindsPerAugmentKillTier4
        {
            get => _windsPerAugmentKillTier4;
            set { if (_windsPerAugmentKillTier4 != value) { _windsPerAugmentKillTier4 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 5", 0f, 100f, "0.##", Order = 4, RequireRestart = false)]
        [SettingPropertyGroup("Buffed unit kills/{UsePerTierAugmentKillRewards}")]
        public float WindsPerAugmentKillTier5
        {
            get => _windsPerAugmentKillTier5;
            set { if (_windsPerAugmentKillTier5 != value) { _windsPerAugmentKillTier5 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 6", 0f, 100f, "0.##", Order = 5, RequireRestart = false)]
        [SettingPropertyGroup("Buffed unit kills/{UsePerTierAugmentKillRewards}")]
        public float WindsPerAugmentKillTier6
        {
            get => _windsPerAugmentKillTier6;
            set { if (_windsPerAugmentKillTier6 != value) { _windsPerAugmentKillTier6 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Winds per heal block", 0f, 100f, "0.##", Order = 0, RequireRestart = false, HintText = "Winds per heal block when a heal spell ends.")]
        [SettingPropertyGroup("Heal rewards", GroupOrder = 2)]
        public float WindsPerHealBlock
        {
            get => _windsPerHealBlock;
            set { if (_windsPerHealBlock != value) { _windsPerHealBlock = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("HP per block", 1f, 10000f, "0.##", Order = 1, RequireRestart = false, HintText = "HP healed per block (default 100 HP = 1 wind).")]
        [SettingPropertyGroup("Heal rewards")]
        public float HealHpPerWind
        {
            get => _healHpPerWind;
            set { if (_healHpPerWind != value) { _healHpPerWind = value; OnPropertyChanged(); } }
        }

        [SettingPropertyBool("Heals count as augments", Order = 2, RequireRestart = false, HintText = "Healed troops also earn buffed-unit kill rewards. Heal-end rewards still apply.")]
        [SettingPropertyGroup("Heal rewards")]
        public bool CountHealSpellsAsAugment
        {
            get => _countHealSpellsAsAugment;
            set { if (_countHealSpellsAsAugment != value) { _countHealSpellsAsAugment = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Winds per second", 0f, 10f, "0.####", Order = 0, RequireRestart = false, HintText = "Passive in-battle regen.")]
        [SettingPropertyGroup("Passive regen", GroupOrder = 3)]
        public float WindsPerSecond
        {
            get => _windsPerSecond;
            set { if (_windsPerSecond != value) { _windsPerSecond = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Winds per damage block", 0f, 100f, "0.##", Order = 0, RequireRestart = false, HintText = "Winds per melee or ranged damage block dealt by you.")]
        [SettingPropertyGroup("Melee and ranged damage", GroupOrder = 4)]
        public float WindsPerMeleeDamageBlock
        {
            get => _windsPerMeleeDamageBlock;
            set { if (_windsPerMeleeDamageBlock != value) { _windsPerMeleeDamageBlock = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("HP per block", 1f, 10000f, "0.##", Order = 1, RequireRestart = false, HintText = "Enemy HP damaged per block (default 100 HP = 1 wind).")]
        [SettingPropertyGroup("Melee and ranged damage")]
        public float MeleeDamageHpPerWind
        {
            get => _meleeDamageHpPerWind;
            set { if (_meleeDamageHpPerWind != value) { _meleeDamageHpPerWind = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Friendly winds per damage block", 0f, 100f, "0.##", Order = 2, RequireRestart = false, HintText = "Winds per melee or ranged damage block dealt to friendly units.")]
        [SettingPropertyGroup("Melee and ranged damage")]
        public float WindsPerFriendlyMeleeDamageBlock
        {
            get => _windsPerFriendlyMeleeDamageBlock;
            set { if (_windsPerFriendlyMeleeDamageBlock != value) { _windsPerFriendlyMeleeDamageBlock = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Winds per damage block", 0f, 100f, "0.##", Order = 0, RequireRestart = false, HintText = "Winds per spell damage block dealt by you.")]
        [SettingPropertyGroup("Spell damage", GroupOrder = 5)]
        public float WindsPerSpellDamageBlock
        {
            get => _windsPerSpellDamageBlock;
            set { if (_windsPerSpellDamageBlock != value) { _windsPerSpellDamageBlock = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("HP per block", 1f, 10000f, "0.##", Order = 1, RequireRestart = false, HintText = "Enemy HP damaged per block (default 100 HP = 1 wind).")]
        [SettingPropertyGroup("Spell damage")]
        public float SpellDamageHpPerWind
        {
            get => _spellDamageHpPerWind;
            set { if (_spellDamageHpPerWind != value) { _spellDamageHpPerWind = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Friendly winds per damage block", 0f, 100f, "0.##", Order = 2, RequireRestart = false, HintText = "Winds per spell damage block dealt to friendly units.")]
        [SettingPropertyGroup("Spell damage")]
        public float WindsPerFriendlySpellDamageBlock
        {
            get => _windsPerFriendlySpellDamageBlock;
            set { if (_windsPerFriendlySpellDamageBlock != value) { _windsPerFriendlySpellDamageBlock = value; OnPropertyChanged(); } }
        }

        public float GetWindsForTier(int tier)
        {
            if (!UsePerTierKillRewards)
                return WindsPerKillAllTiers;

            return GetWindsForTier(
                tier,
                WindsPerKillTier1,
                WindsPerKillTier2,
                WindsPerKillTier3,
                WindsPerKillTier4,
                WindsPerKillTier5,
                WindsPerKillTier6);
        }

        public float GetWindsForAugmentKillTier(int tier)
        {
            if (!UsePerTierAugmentKillRewards)
                return WindsPerAugmentKillAllTiers;

            return GetWindsForTier(
                tier,
                WindsPerAugmentKillTier1,
                WindsPerAugmentKillTier2,
                WindsPerAugmentKillTier3,
                WindsPerAugmentKillTier4,
                WindsPerAugmentKillTier5,
                WindsPerAugmentKillTier6);
        }

        private static float GetWindsForTier(
            int tier,
            float tier1,
            float tier2,
            float tier3,
            float tier4,
            float tier5,
            float tier6)
        {
            return tier switch
            {
                1 => tier1,
                2 => tier2,
                3 => tier3,
                4 => tier4,
                5 => tier5,
                6 => tier6,
                _ => tier1,
            };
        }

        public float GetWindsForHealing(int totalHpHealed)
        {
            if (totalHpHealed <= 0 || HealHpPerWind <= 0f || WindsPerHealBlock <= 0f)
                return 0f;

            return totalHpHealed / HealHpPerWind * WindsPerHealBlock;
        }

        public float GetWindsForMeleeDamage(float damageDealt)
        {
            return GetWindsForDamage(damageDealt, MeleeDamageHpPerWind, WindsPerMeleeDamageBlock);
        }

        public float GetWindsForSpellDamage(int damageDealt)
        {
            return GetWindsForDamage(damageDealt, SpellDamageHpPerWind, WindsPerSpellDamageBlock);
        }

        public float GetWindsForFriendlyMeleeDamage(float damageDealt)
        {
            return GetWindsForDamage(damageDealt, MeleeDamageHpPerWind, WindsPerFriendlyMeleeDamageBlock);
        }

        public float GetWindsForFriendlySpellDamage(int damageDealt)
        {
            return GetWindsForDamage(damageDealt, SpellDamageHpPerWind, WindsPerFriendlySpellDamageBlock);
        }

        private static float GetWindsForDamage(float damageDealt, float hpPerWind, float windsPerBlock)
        {
            if (damageDealt <= 0f || hpPerWind <= 0f || windsPerBlock <= 0f)
                return 0f;

            return damageDealt / hpPerWind * windsPerBlock;
        }
    }
}
