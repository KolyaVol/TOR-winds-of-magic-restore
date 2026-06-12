using System;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using WindsOfMagicRestore.Utilities;

namespace WindsOfMagicRestore.Settings
{
    public sealed class WindsOfMagicRestoreSettings : AttributeGlobalSettings<WindsOfMagicRestoreSettings>
    {
        private bool _enableKillRewards = true;
        private float _windsPerKillAllTiers = 1f;
        private bool _usePerTierKillRewards;
        private float _windsPerKillTier1 = 1f;
        private float _windsPerKillTier2;
        private float _windsPerKillTier3;
        private float _windsPerKillTier4;
        private float _windsPerKillTier5;
        private float _windsPerKillTier6;
        private float _windsPerKillTier7;
        private float _windsPerKillTier8;
        private float _windsPerKillTier9;
        private bool _enableAugmentKillRewards = true;
        private float _windsPerAugmentKillAllTiers = 1f;
        private bool _usePerTierAugmentKillRewards;
        private float _windsPerAugmentKillTier1 = 1f;
        private float _windsPerAugmentKillTier2;
        private float _windsPerAugmentKillTier3;
        private float _windsPerAugmentKillTier4;
        private float _windsPerAugmentKillTier5;
        private float _windsPerAugmentKillTier6;
        private float _windsPerAugmentKillTier7;
        private float _windsPerAugmentKillTier8;
        private float _windsPerAugmentKillTier9;
        private float _windsPerHealBlock = 1f;
        private float _healHpPerWind = 100f;
        private bool _countHealSpellsAsAugment;
        private float _windsPerSecond = 0.05f;
        private float _windsPerMeleeDamageBlock;
        private float _windsPerFriendlyMeleeDamageBlock;
        private float _windsPerSpellDamageBlock;
        private float _windsPerFriendlySpellDamageBlock;

        private const float DamageHpPerWindBlock = 100f;
        private bool _showBattleDiagnosticsWarning = true;

        public override string Id => "WindsOfMagicRestore_v1";
        public override string DisplayName => "Winds of Magic Restore";
        public override string FolderName => "WindsOfMagicRestore";
        public override string FormatType => "json";

        public static void Initialize()
        {
            ModTrace.Mark("settings:initialize:start");
            try
            {
                _ = Instance;
                ModTrace.Mark("settings:initialize:ok");
            }
            catch (Exception ex)
            {
                ModTrace.Mark("settings:initialize:failed", ex);
                ModLog.Error($"MCM settings failed to load — {ModLog.FormatException(ex)}");
            }
        }

        public string EnableKillRewardsLabel => ToggleStatus(EnableKillRewards);

        [SettingPropertyBool("Enable kill rewards ({EnableKillRewardsLabel})", IsToggle = true, Order = 0, RequireRestart = false, HintText = "Grant winds when you get kill credit.")]
        [SettingPropertyGroup("Kill rewards", GroupOrder = 0)]
        public bool EnableKillRewards
        {
            get => _enableKillRewards;
            set
            {
                if (_enableKillRewards == value)
                    return;

                _enableKillRewards = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EnableKillRewardsLabel));
                NotifyKillRewardGroupVisibilityChanged();
            }
        }

        public bool ShowKillRewardOptions => EnableKillRewards;

        public bool ShowUniformKillReward => EnableKillRewards && !UsePerTierKillRewards;

        public bool ShowPerTierKillRewards => EnableKillRewards && UsePerTierKillRewards;

        [SettingPropertyFloatingInteger("Winds per kill", 0f, 100f, "0.##", Order = 0, RequireRestart = false, HintText = "Used for every enemy tier unless per-tier rewards are enabled below.")]
        [SettingPropertyGroup("Kill rewards/{ShowUniformKillReward}")]
        public float WindsPerKillAllTiers
        {
            get => _windsPerKillAllTiers;
            set { if (_windsPerKillAllTiers != value) { _windsPerKillAllTiers = value; OnPropertyChanged(); } }
        }

        public string UsePerTierKillRewardsLabel => ToggleStatus(UsePerTierKillRewards);

        [SettingPropertyBool("Set different winds gain for every tier ({UsePerTierKillRewardsLabel})", IsToggle = true, Order = 1, RequireRestart = false, HintText = "Use the tier values below instead of winds per kill.")]
        [SettingPropertyGroup("Kill rewards/{ShowKillRewardOptions}")]
        public bool UsePerTierKillRewards
        {
            get => _usePerTierKillRewards;
            set
            {
                if (_usePerTierKillRewards == value)
                    return;

                _usePerTierKillRewards = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(UsePerTierKillRewardsLabel));
                OnPropertyChanged(nameof(ShowUniformKillReward));
                OnPropertyChanged(nameof(ShowPerTierKillRewards));
            }
        }

        [SettingPropertyFloatingInteger("Tier 1", 0f, 100f, "0.##", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards/{ShowPerTierKillRewards}")]
        public float WindsPerKillTier1
        {
            get => _windsPerKillTier1;
            set { if (_windsPerKillTier1 != value) { _windsPerKillTier1 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 2", 0f, 100f, "0.##", Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards/{ShowPerTierKillRewards}")]
        public float WindsPerKillTier2
        {
            get => _windsPerKillTier2;
            set { if (_windsPerKillTier2 != value) { _windsPerKillTier2 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 3", 0f, 100f, "0.##", Order = 2, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards/{ShowPerTierKillRewards}")]
        public float WindsPerKillTier3
        {
            get => _windsPerKillTier3;
            set { if (_windsPerKillTier3 != value) { _windsPerKillTier3 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 4", 0f, 100f, "0.##", Order = 3, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards/{ShowPerTierKillRewards}")]
        public float WindsPerKillTier4
        {
            get => _windsPerKillTier4;
            set { if (_windsPerKillTier4 != value) { _windsPerKillTier4 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 5", 0f, 100f, "0.##", Order = 4, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards/{ShowPerTierKillRewards}")]
        public float WindsPerKillTier5
        {
            get => _windsPerKillTier5;
            set { if (_windsPerKillTier5 != value) { _windsPerKillTier5 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 6", 0f, 100f, "0.##", Order = 5, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards/{ShowPerTierKillRewards}")]
        public float WindsPerKillTier6
        {
            get => _windsPerKillTier6;
            set { if (_windsPerKillTier6 != value) { _windsPerKillTier6 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 7", 0f, 100f, "0.##", Order = 6, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards/{ShowPerTierKillRewards}")]
        public float WindsPerKillTier7
        {
            get => _windsPerKillTier7;
            set { if (_windsPerKillTier7 != value) { _windsPerKillTier7 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 8", 0f, 100f, "0.##", Order = 7, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards/{ShowPerTierKillRewards}")]
        public float WindsPerKillTier8
        {
            get => _windsPerKillTier8;
            set { if (_windsPerKillTier8 != value) { _windsPerKillTier8 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 9", 0f, 100f, "0.##", Order = 8, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards/{ShowPerTierKillRewards}")]
        public float WindsPerKillTier9
        {
            get => _windsPerKillTier9;
            set { if (_windsPerKillTier9 != value) { _windsPerKillTier9 = value; OnPropertyChanged(); } }
        }

        public string EnableAugmentKillRewardsLabel => ToggleStatus(EnableAugmentKillRewards);

        [SettingPropertyBool("Enable buffed unit kill rewards ({EnableAugmentKillRewardsLabel})", IsToggle = true, Order = 0, RequireRestart = false, HintText = "Grant winds when a troop you buffed kills an enemy.")]
        [SettingPropertyGroup("Buffed unit kills", GroupOrder = 1)]
        public bool EnableAugmentKillRewards
        {
            get => _enableAugmentKillRewards;
            set
            {
                if (_enableAugmentKillRewards == value)
                    return;

                _enableAugmentKillRewards = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EnableAugmentKillRewardsLabel));
                NotifyAugmentKillRewardGroupVisibilityChanged();
            }
        }

        public bool ShowAugmentKillOptions => EnableAugmentKillRewards;

        public bool ShowUniformAugmentKillReward => EnableAugmentKillRewards && !UsePerTierAugmentKillRewards;

        public bool ShowPerTierAugmentKillRewards => EnableAugmentKillRewards && UsePerTierAugmentKillRewards;

        [SettingPropertyFloatingInteger("Winds per kill", 0f, 100f, "0.##", Order = 0, RequireRestart = false, HintText = "Used for every enemy tier unless per-tier rewards are enabled below.")]
        [SettingPropertyGroup("Buffed unit kills/{ShowUniformAugmentKillReward}")]
        public float WindsPerAugmentKillAllTiers
        {
            get => _windsPerAugmentKillAllTiers;
            set { if (_windsPerAugmentKillAllTiers != value) { _windsPerAugmentKillAllTiers = value; OnPropertyChanged(); } }
        }

        public string UsePerTierAugmentKillRewardsLabel => ToggleStatus(UsePerTierAugmentKillRewards);

        [SettingPropertyBool("Set different winds gain for every tier ({UsePerTierAugmentKillRewardsLabel})", IsToggle = true, Order = 1, RequireRestart = false, HintText = "Use the tier values below instead of winds per kill.")]
        [SettingPropertyGroup("Buffed unit kills/{ShowAugmentKillOptions}")]
        public bool UsePerTierAugmentKillRewards
        {
            get => _usePerTierAugmentKillRewards;
            set
            {
                if (_usePerTierAugmentKillRewards == value)
                    return;

                _usePerTierAugmentKillRewards = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(UsePerTierAugmentKillRewardsLabel));
                OnPropertyChanged(nameof(ShowUniformAugmentKillReward));
                OnPropertyChanged(nameof(ShowPerTierAugmentKillRewards));
            }
        }

        [SettingPropertyFloatingInteger("Tier 1", 0f, 100f, "0.##", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Buffed unit kills/{ShowPerTierAugmentKillRewards}")]
        public float WindsPerAugmentKillTier1
        {
            get => _windsPerAugmentKillTier1;
            set { if (_windsPerAugmentKillTier1 != value) { _windsPerAugmentKillTier1 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 2", 0f, 100f, "0.##", Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("Buffed unit kills/{ShowPerTierAugmentKillRewards}")]
        public float WindsPerAugmentKillTier2
        {
            get => _windsPerAugmentKillTier2;
            set { if (_windsPerAugmentKillTier2 != value) { _windsPerAugmentKillTier2 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 3", 0f, 100f, "0.##", Order = 2, RequireRestart = false)]
        [SettingPropertyGroup("Buffed unit kills/{ShowPerTierAugmentKillRewards}")]
        public float WindsPerAugmentKillTier3
        {
            get => _windsPerAugmentKillTier3;
            set { if (_windsPerAugmentKillTier3 != value) { _windsPerAugmentKillTier3 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 4", 0f, 100f, "0.##", Order = 3, RequireRestart = false)]
        [SettingPropertyGroup("Buffed unit kills/{ShowPerTierAugmentKillRewards}")]
        public float WindsPerAugmentKillTier4
        {
            get => _windsPerAugmentKillTier4;
            set { if (_windsPerAugmentKillTier4 != value) { _windsPerAugmentKillTier4 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 5", 0f, 100f, "0.##", Order = 4, RequireRestart = false)]
        [SettingPropertyGroup("Buffed unit kills/{ShowPerTierAugmentKillRewards}")]
        public float WindsPerAugmentKillTier5
        {
            get => _windsPerAugmentKillTier5;
            set { if (_windsPerAugmentKillTier5 != value) { _windsPerAugmentKillTier5 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 6", 0f, 100f, "0.##", Order = 5, RequireRestart = false)]
        [SettingPropertyGroup("Buffed unit kills/{ShowPerTierAugmentKillRewards}")]
        public float WindsPerAugmentKillTier6
        {
            get => _windsPerAugmentKillTier6;
            set { if (_windsPerAugmentKillTier6 != value) { _windsPerAugmentKillTier6 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 7", 0f, 100f, "0.##", Order = 6, RequireRestart = false)]
        [SettingPropertyGroup("Buffed unit kills/{ShowPerTierAugmentKillRewards}")]
        public float WindsPerAugmentKillTier7
        {
            get => _windsPerAugmentKillTier7;
            set { if (_windsPerAugmentKillTier7 != value) { _windsPerAugmentKillTier7 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 8", 0f, 100f, "0.##", Order = 7, RequireRestart = false)]
        [SettingPropertyGroup("Buffed unit kills/{ShowPerTierAugmentKillRewards}")]
        public float WindsPerAugmentKillTier8
        {
            get => _windsPerAugmentKillTier8;
            set { if (_windsPerAugmentKillTier8 != value) { _windsPerAugmentKillTier8 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 9", 0f, 100f, "0.##", Order = 8, RequireRestart = false)]
        [SettingPropertyGroup("Buffed unit kills/{ShowPerTierAugmentKillRewards}")]
        public float WindsPerAugmentKillTier9
        {
            get => _windsPerAugmentKillTier9;
            set { if (_windsPerAugmentKillTier9 != value) { _windsPerAugmentKillTier9 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Winds per selected amount HP", 0f, 100f, "0.##", Order = 0, RequireRestart = false, HintText = "Winds restored when a heal spell ends. Total payout = (HP healed ÷ HP block for winds) × this value. Set to 0 to disable.")]
        [SettingPropertyGroup("Heal rewards", GroupOrder = 2)]
        public float WindsPerHealBlock
        {
            get => _windsPerHealBlock;
            set { if (_windsPerHealBlock != value) { _windsPerHealBlock = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("HP block for winds", 1f, 10000f, "0.##", Order = 1, RequireRestart = false, HintText = "How much HP must be restored for one wind payout. Default 100 with 1.0 winds = 1 wind per 100 HP healed.")]
        [SettingPropertyGroup("Heal rewards")]
        public float HealHpPerWind
        {
            get => _healHpPerWind;
            set { if (_healHpPerWind != value) { _healHpPerWind = value; OnPropertyChanged(); } }
        }

        public string CountHealSpellsAsAugmentLabel => ToggleStatus(CountHealSpellsAsAugment);

        [SettingPropertyBool("Heals count as augments ({CountHealSpellsAsAugmentLabel})", Order = 2, RequireRestart = false, HintText = "Healed allies count as buffed units for kill rewards. Winds from healing are unchanged.")]
        [SettingPropertyGroup("Heal rewards")]
        public bool CountHealSpellsAsAugment
        {
            get => _countHealSpellsAsAugment;
            set
            {
                if (_countHealSpellsAsAugment == value)
                    return;

                _countHealSpellsAsAugment = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CountHealSpellsAsAugmentLabel));
            }
        }

        [SettingPropertyFloatingInteger("Winds per second", 0f, 10f, "0.####", Order = 0, RequireRestart = false, HintText = "Passive in-battle regen.")]
        [SettingPropertyGroup("Passive regen", GroupOrder = 3)]
        public float WindsPerSecond
        {
            get => _windsPerSecond;
            set { if (_windsPerSecond != value) { _windsPerSecond = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Winds for 100 damage", 0f, 100f, "0.##", Order = 0, RequireRestart = false, HintText = "Winds restored per 100 HP of melee or ranged damage dealt. Scales proportionally (50 HP = half this value). Set to 0 to disable.")]
        [SettingPropertyGroup("Melee and ranged damage", GroupOrder = 4)]
        public float WindsPerMeleeDamageBlock
        {
            get => _windsPerMeleeDamageBlock;
            set { if (_windsPerMeleeDamageBlock != value) { _windsPerMeleeDamageBlock = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Winds for friendly fire 100 damage", 0f, 100f, "0.##", Order = 1, RequireRestart = false, HintText = "Winds restored per 100 HP of melee or ranged damage dealt to friendly units. Scales proportionally.")]
        [SettingPropertyGroup("Melee and ranged damage")]
        public float WindsPerFriendlyMeleeDamageBlock
        {
            get => _windsPerFriendlyMeleeDamageBlock;
            set { if (_windsPerFriendlyMeleeDamageBlock != value) { _windsPerFriendlyMeleeDamageBlock = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Winds for 100 damage", 0f, 100f, "0.##", Order = 0, RequireRestart = false, HintText = "Winds restored per 100 HP of spell damage dealt. Scales proportionally (50 HP = half this value). Set to 0 to disable.")]
        [SettingPropertyGroup("Spell damage", GroupOrder = 5)]
        public float WindsPerSpellDamageBlock
        {
            get => _windsPerSpellDamageBlock;
            set { if (_windsPerSpellDamageBlock != value) { _windsPerSpellDamageBlock = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Winds for friendly fire 100 damage", 0f, 100f, "0.##", Order = 1, RequireRestart = false, HintText = "Winds restored per 100 HP of spell damage dealt to friendly units. Scales proportionally.")]
        [SettingPropertyGroup("Spell damage")]
        public float WindsPerFriendlySpellDamageBlock
        {
            get => _windsPerFriendlySpellDamageBlock;
            set { if (_windsPerFriendlySpellDamageBlock != value) { _windsPerFriendlySpellDamageBlock = value; OnPropertyChanged(); } }
        }

        public string ShowBattleDiagnosticsWarningLabel => ToggleStatus(ShowBattleDiagnosticsWarning);

        [SettingPropertyBool(
            "Warn at battle start ({ShowBattleDiagnosticsWarningLabel})",
            Order = 0,
            RequireRestart = false,
            HintText = "Show a one-time on-screen message when compatibility issues are detected. Full report: wom.diagnostics in battle console.")]
        [SettingPropertyGroup("Diagnostics", GroupOrder = 7)]
        public bool ShowBattleDiagnosticsWarning
        {
            get => _showBattleDiagnosticsWarning;
            set
            {
                if (_showBattleDiagnosticsWarning == value)
                    return;

                _showBattleDiagnosticsWarning = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowBattleDiagnosticsWarningLabel));
            }
        }

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

        private static float GetWindsForDamage(float damageDealt, float hpPerWind, float windsPerBlock)
        {
            if (damageDealt <= 0f || hpPerWind <= 0f || windsPerBlock <= 0f)
                return 0f;

            return damageDealt / hpPerWind * windsPerBlock;
        }

        private void NotifyKillRewardGroupVisibilityChanged()
        {
            OnPropertyChanged(nameof(ShowKillRewardOptions));
            OnPropertyChanged(nameof(ShowUniformKillReward));
            OnPropertyChanged(nameof(ShowPerTierKillRewards));
        }

        private void NotifyAugmentKillRewardGroupVisibilityChanged()
        {
            OnPropertyChanged(nameof(ShowAugmentKillOptions));
            OnPropertyChanged(nameof(ShowUniformAugmentKillReward));
            OnPropertyChanged(nameof(ShowPerTierAugmentKillRewards));
        }

        private static string ToggleStatus(bool enabled) => enabled ? "Enabled" : "Disabled";
    }
}
