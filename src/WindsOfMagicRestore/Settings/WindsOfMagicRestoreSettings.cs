using System;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using MCM.Common;
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
        private bool _enableCompanionKillRewards = true;
        private Dropdown<CompanionWindsRecipientOption> _companionKillRestoreTarget = CompanionWindsRecipientOption.CreateDefaultDropdown();
        private float _companionWindsPerKillAllTiers = 1f;
        private bool _companionUsePerTierKillRewards;
        private float _companionWindsPerKillTier1 = 1f;
        private float _companionWindsPerKillTier2;
        private float _companionWindsPerKillTier3;
        private float _companionWindsPerKillTier4;
        private float _companionWindsPerKillTier5;
        private float _companionWindsPerKillTier6;
        private float _companionWindsPerKillTier7;
        private float _companionWindsPerKillTier8;
        private float _companionWindsPerKillTier9;
        private bool _enableCompanionHealRewards = true;
        private Dropdown<CompanionWindsRecipientOption> _companionHealRestoreTarget = CompanionWindsRecipientOption.CreateDefaultDropdown();
        private float _companionWindsPerHealBlock = 1f;
        private float _companionHealHpPerWind = 100f;
        private Dropdown<CompanionWindsRecipientOption> _companionDamageRestoreTarget = CompanionWindsRecipientOption.CreateDefaultDropdown();
        private float _companionWindsPerMeleeDamageBlock;
        private float _companionWindsPerFriendlyMeleeDamageBlock;
        private float _companionWindsPerSpellDamageBlock;
        private float _companionWindsPerFriendlySpellDamageBlock;

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

        [SettingPropertyBool("Enable kill rewards", IsToggle = true, Order = 0, RequireRestart = false, HintText = "Grant winds when you get kill credit.")]
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
                NotifyKillRewardGroupVisibilityChanged();
            }
        }

        public string KillRewardOptionsGroup => EnableKillRewards ? "Settings" : string.Empty;

        public string UniformKillRewardGroup =>
            EnableKillRewards && !UsePerTierKillRewards ? "All enemy tiers" : string.Empty;

        public string PerTierKillRewardsGroup =>
            EnableKillRewards && UsePerTierKillRewards ? "Per enemy tier" : string.Empty;

        [SettingPropertyFloatingInteger("Winds per kill", 0f, 100f, "0.##", Order = 0, RequireRestart = false, HintText = "Used for every enemy tier unless per-tier rewards are enabled below.")]
        [SettingPropertyGroup("Kill rewards/{UniformKillRewardGroup}")]
        public float WindsPerKillAllTiers
        {
            get => _windsPerKillAllTiers;
            set { if (_windsPerKillAllTiers != value) { _windsPerKillAllTiers = value; OnPropertyChanged(); } }
        }

        [SettingPropertyBool("Set different winds gain for every tier", Order = 1, RequireRestart = false, HintText = "When on, set winds for each tier 1–9 below. When off, one winds-per-kill value applies to all tiers.")]
        [SettingPropertyGroup("Kill rewards/{KillRewardOptionsGroup}")]
        public bool UsePerTierKillRewards
        {
            get => _usePerTierKillRewards;
            set
            {
                if (_usePerTierKillRewards == value)
                    return;

                _usePerTierKillRewards = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(UniformKillRewardGroup));
                OnPropertyChanged(nameof(PerTierKillRewardsGroup));
            }
        }

        [SettingPropertyFloatingInteger("Tier 1", 0f, 100f, "0.##", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards/{PerTierKillRewardsGroup}")]
        public float WindsPerKillTier1
        {
            get => _windsPerKillTier1;
            set { if (_windsPerKillTier1 != value) { _windsPerKillTier1 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 2", 0f, 100f, "0.##", Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards/{PerTierKillRewardsGroup}")]
        public float WindsPerKillTier2
        {
            get => _windsPerKillTier2;
            set { if (_windsPerKillTier2 != value) { _windsPerKillTier2 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 3", 0f, 100f, "0.##", Order = 2, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards/{PerTierKillRewardsGroup}")]
        public float WindsPerKillTier3
        {
            get => _windsPerKillTier3;
            set { if (_windsPerKillTier3 != value) { _windsPerKillTier3 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 4", 0f, 100f, "0.##", Order = 3, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards/{PerTierKillRewardsGroup}")]
        public float WindsPerKillTier4
        {
            get => _windsPerKillTier4;
            set { if (_windsPerKillTier4 != value) { _windsPerKillTier4 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 5", 0f, 100f, "0.##", Order = 4, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards/{PerTierKillRewardsGroup}")]
        public float WindsPerKillTier5
        {
            get => _windsPerKillTier5;
            set { if (_windsPerKillTier5 != value) { _windsPerKillTier5 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 6", 0f, 100f, "0.##", Order = 5, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards/{PerTierKillRewardsGroup}")]
        public float WindsPerKillTier6
        {
            get => _windsPerKillTier6;
            set { if (_windsPerKillTier6 != value) { _windsPerKillTier6 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 7", 0f, 100f, "0.##", Order = 6, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards/{PerTierKillRewardsGroup}")]
        public float WindsPerKillTier7
        {
            get => _windsPerKillTier7;
            set { if (_windsPerKillTier7 != value) { _windsPerKillTier7 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 8", 0f, 100f, "0.##", Order = 7, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards/{PerTierKillRewardsGroup}")]
        public float WindsPerKillTier8
        {
            get => _windsPerKillTier8;
            set { if (_windsPerKillTier8 != value) { _windsPerKillTier8 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 9", 0f, 100f, "0.##", Order = 8, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards/{PerTierKillRewardsGroup}")]
        public float WindsPerKillTier9
        {
            get => _windsPerKillTier9;
            set { if (_windsPerKillTier9 != value) { _windsPerKillTier9 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyBool("Enable buffed unit kill rewards", IsToggle = true, Order = 0, RequireRestart = false, HintText = "Grant winds when a troop you buffed kills an enemy.")]
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
                NotifyAugmentKillRewardGroupVisibilityChanged();
            }
        }

        public string AugmentKillOptionsGroup => EnableAugmentKillRewards ? "Settings" : string.Empty;

        public string UniformAugmentKillRewardGroup =>
            EnableAugmentKillRewards && !UsePerTierAugmentKillRewards ? "All enemy tiers" : string.Empty;

        public string PerTierAugmentKillRewardsGroup =>
            EnableAugmentKillRewards && UsePerTierAugmentKillRewards ? "Per enemy tier" : string.Empty;

        [SettingPropertyFloatingInteger("Winds per kill", 0f, 100f, "0.##", Order = 0, RequireRestart = false, HintText = "Used for every enemy tier unless per-tier rewards are enabled below.")]
        [SettingPropertyGroup("Buffed unit kills/{UniformAugmentKillRewardGroup}")]
        public float WindsPerAugmentKillAllTiers
        {
            get => _windsPerAugmentKillAllTiers;
            set { if (_windsPerAugmentKillAllTiers != value) { _windsPerAugmentKillAllTiers = value; OnPropertyChanged(); } }
        }

        [SettingPropertyBool("Set different winds gain for every tier", Order = 1, RequireRestart = false, HintText = "When on, set winds for each tier 1–9 below. When off, one winds-per-kill value applies to all tiers.")]
        [SettingPropertyGroup("Buffed unit kills/{AugmentKillOptionsGroup}")]
        public bool UsePerTierAugmentKillRewards
        {
            get => _usePerTierAugmentKillRewards;
            set
            {
                if (_usePerTierAugmentKillRewards == value)
                    return;

                _usePerTierAugmentKillRewards = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(UniformAugmentKillRewardGroup));
                OnPropertyChanged(nameof(PerTierAugmentKillRewardsGroup));
            }
        }

        [SettingPropertyFloatingInteger("Tier 1", 0f, 100f, "0.##", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Buffed unit kills/{PerTierAugmentKillRewardsGroup}")]
        public float WindsPerAugmentKillTier1
        {
            get => _windsPerAugmentKillTier1;
            set { if (_windsPerAugmentKillTier1 != value) { _windsPerAugmentKillTier1 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 2", 0f, 100f, "0.##", Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("Buffed unit kills/{PerTierAugmentKillRewardsGroup}")]
        public float WindsPerAugmentKillTier2
        {
            get => _windsPerAugmentKillTier2;
            set { if (_windsPerAugmentKillTier2 != value) { _windsPerAugmentKillTier2 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 3", 0f, 100f, "0.##", Order = 2, RequireRestart = false)]
        [SettingPropertyGroup("Buffed unit kills/{PerTierAugmentKillRewardsGroup}")]
        public float WindsPerAugmentKillTier3
        {
            get => _windsPerAugmentKillTier3;
            set { if (_windsPerAugmentKillTier3 != value) { _windsPerAugmentKillTier3 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 4", 0f, 100f, "0.##", Order = 3, RequireRestart = false)]
        [SettingPropertyGroup("Buffed unit kills/{PerTierAugmentKillRewardsGroup}")]
        public float WindsPerAugmentKillTier4
        {
            get => _windsPerAugmentKillTier4;
            set { if (_windsPerAugmentKillTier4 != value) { _windsPerAugmentKillTier4 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 5", 0f, 100f, "0.##", Order = 4, RequireRestart = false)]
        [SettingPropertyGroup("Buffed unit kills/{PerTierAugmentKillRewardsGroup}")]
        public float WindsPerAugmentKillTier5
        {
            get => _windsPerAugmentKillTier5;
            set { if (_windsPerAugmentKillTier5 != value) { _windsPerAugmentKillTier5 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 6", 0f, 100f, "0.##", Order = 5, RequireRestart = false)]
        [SettingPropertyGroup("Buffed unit kills/{PerTierAugmentKillRewardsGroup}")]
        public float WindsPerAugmentKillTier6
        {
            get => _windsPerAugmentKillTier6;
            set { if (_windsPerAugmentKillTier6 != value) { _windsPerAugmentKillTier6 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 7", 0f, 100f, "0.##", Order = 6, RequireRestart = false)]
        [SettingPropertyGroup("Buffed unit kills/{PerTierAugmentKillRewardsGroup}")]
        public float WindsPerAugmentKillTier7
        {
            get => _windsPerAugmentKillTier7;
            set { if (_windsPerAugmentKillTier7 != value) { _windsPerAugmentKillTier7 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 8", 0f, 100f, "0.##", Order = 7, RequireRestart = false)]
        [SettingPropertyGroup("Buffed unit kills/{PerTierAugmentKillRewardsGroup}")]
        public float WindsPerAugmentKillTier8
        {
            get => _windsPerAugmentKillTier8;
            set { if (_windsPerAugmentKillTier8 != value) { _windsPerAugmentKillTier8 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 9", 0f, 100f, "0.##", Order = 8, RequireRestart = false)]
        [SettingPropertyGroup("Buffed unit kills/{PerTierAugmentKillRewardsGroup}")]
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

        [SettingPropertyBool("Heals count as augments", Order = 2, RequireRestart = false, HintText = "Healed allies count as buffed units for kill rewards. Winds from healing are unchanged.")]
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
            }
        }

        [SettingPropertyFloatingInteger("Winds per second", 0f, 10f, "0.####", Order = 0, RequireRestart = false, HintText = "Passive in-battle regen.")]
        [SettingPropertyGroup("Passive regen", GroupOrder = 3)]
        public float WindsPerSecond
        {
            get => _windsPerSecond;
            set { if (_windsPerSecond != value) { _windsPerSecond = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Winds per 100 HP dealt", 0f, 100f, "0.##", Order = 0, RequireRestart = false, HintText = "Winds restored per 100 HP of melee or ranged damage dealt. Scales proportionally (50 HP = half this value). Set to 0 to disable.")]
        [SettingPropertyGroup("Melee and ranged damage", GroupOrder = 4)]
        public float WindsPerMeleeDamageBlock
        {
            get => _windsPerMeleeDamageBlock;
            set { if (_windsPerMeleeDamageBlock != value) { _windsPerMeleeDamageBlock = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Friendly fire winds per 100 HP", 0f, 100f, "0.##", Order = 1, RequireRestart = false, HintText = "Winds restored per 100 HP of melee or ranged damage dealt to friendly units. Scales proportionally.")]
        [SettingPropertyGroup("Melee and ranged damage")]
        public float WindsPerFriendlyMeleeDamageBlock
        {
            get => _windsPerFriendlyMeleeDamageBlock;
            set { if (_windsPerFriendlyMeleeDamageBlock != value) { _windsPerFriendlyMeleeDamageBlock = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Winds per 100 HP dealt", 0f, 100f, "0.##", Order = 0, RequireRestart = false, HintText = "Winds restored per 100 HP of spell damage dealt. Scales proportionally (50 HP = half this value). Set to 0 to disable.")]
        [SettingPropertyGroup("Spell damage", GroupOrder = 5)]
        public float WindsPerSpellDamageBlock
        {
            get => _windsPerSpellDamageBlock;
            set { if (_windsPerSpellDamageBlock != value) { _windsPerSpellDamageBlock = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Friendly fire winds per 100 HP", 0f, 100f, "0.##", Order = 1, RequireRestart = false, HintText = "Winds restored per 100 HP of spell damage dealt to friendly units. Scales proportionally.")]
        [SettingPropertyGroup("Spell damage")]
        public float WindsPerFriendlySpellDamageBlock
        {
            get => _windsPerFriendlySpellDamageBlock;
            set { if (_windsPerFriendlySpellDamageBlock != value) { _windsPerFriendlySpellDamageBlock = value; OnPropertyChanged(); } }
        }

        [SettingPropertyBool("Enable companion kill rewards", IsToggle = true, Order = 0, RequireRestart = false, HintText = "Grant winds when a party companion gets kill credit.")]
        [SettingPropertyGroup("Companion kill rewards", GroupOrder = 6)]
        public bool EnableCompanionKillRewards
        {
            get => _enableCompanionKillRewards;
            set
            {
                if (_enableCompanionKillRewards == value)
                    return;

                _enableCompanionKillRewards = value;
                OnPropertyChanged();
                NotifyCompanionKillRewardGroupVisibilityChanged();
            }
        }

        public string CompanionKillRewardOptionsGroup => EnableCompanionKillRewards ? string.Empty : "!";

        public string UniformCompanionKillRewardGroup =>
            EnableCompanionKillRewards && !CompanionUsePerTierKillRewards ? "All enemy tiers" : string.Empty;

        public string PerTierCompanionKillRewardsGroup =>
            EnableCompanionKillRewards && CompanionUsePerTierKillRewards ? "Per enemy tier" : string.Empty;

        [SettingPropertyDropdown("Restore target", Order = 1, RequireRestart = false, HintText = "Who receives companion kill winds. Main hero, Self, or Both.")]
        [SettingPropertyGroup("Companion kill rewards")]
        public Dropdown<CompanionWindsRecipientOption> CompanionKillRestoreTarget
        {
            get => _companionKillRestoreTarget;
            set
            {
                if (_companionKillRestoreTarget == value)
                    return;

                _companionKillRestoreTarget = value;
                OnPropertyChanged();
            }
        }

        [SettingPropertyFloatingInteger("Winds per kill", 0f, 100f, "0.##", Order = 2, RequireRestart = false, HintText = "Used for every enemy tier unless per-tier rewards are enabled below.")]
        [SettingPropertyGroup("Companion kill rewards/{UniformCompanionKillRewardGroup}")]
        public float CompanionWindsPerKillAllTiers
        {
            get => _companionWindsPerKillAllTiers;
            set { if (_companionWindsPerKillAllTiers != value) { _companionWindsPerKillAllTiers = value; OnPropertyChanged(); } }
        }

        [SettingPropertyBool("Set different winds gain for every tier", Order = 3, RequireRestart = false, HintText = "When on, set winds for each tier 1–9 below. When off, one winds-per-kill value applies to all tiers.")]
        [SettingPropertyGroup("Companion kill rewards/{CompanionKillRewardOptionsGroup}")]
        public bool CompanionUsePerTierKillRewards
        {
            get => _companionUsePerTierKillRewards;
            set
            {
                if (_companionUsePerTierKillRewards == value)
                    return;

                _companionUsePerTierKillRewards = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(UniformCompanionKillRewardGroup));
                OnPropertyChanged(nameof(PerTierCompanionKillRewardsGroup));
            }
        }

        [SettingPropertyFloatingInteger("Tier 1", 0f, 100f, "0.##", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Companion kill rewards/{PerTierCompanionKillRewardsGroup}")]
        public float CompanionWindsPerKillTier1
        {
            get => _companionWindsPerKillTier1;
            set { if (_companionWindsPerKillTier1 != value) { _companionWindsPerKillTier1 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 2", 0f, 100f, "0.##", Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("Companion kill rewards/{PerTierCompanionKillRewardsGroup}")]
        public float CompanionWindsPerKillTier2
        {
            get => _companionWindsPerKillTier2;
            set { if (_companionWindsPerKillTier2 != value) { _companionWindsPerKillTier2 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 3", 0f, 100f, "0.##", Order = 2, RequireRestart = false)]
        [SettingPropertyGroup("Companion kill rewards/{PerTierCompanionKillRewardsGroup}")]
        public float CompanionWindsPerKillTier3
        {
            get => _companionWindsPerKillTier3;
            set { if (_companionWindsPerKillTier3 != value) { _companionWindsPerKillTier3 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 4", 0f, 100f, "0.##", Order = 3, RequireRestart = false)]
        [SettingPropertyGroup("Companion kill rewards/{PerTierCompanionKillRewardsGroup}")]
        public float CompanionWindsPerKillTier4
        {
            get => _companionWindsPerKillTier4;
            set { if (_companionWindsPerKillTier4 != value) { _companionWindsPerKillTier4 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 5", 0f, 100f, "0.##", Order = 4, RequireRestart = false)]
        [SettingPropertyGroup("Companion kill rewards/{PerTierCompanionKillRewardsGroup}")]
        public float CompanionWindsPerKillTier5
        {
            get => _companionWindsPerKillTier5;
            set { if (_companionWindsPerKillTier5 != value) { _companionWindsPerKillTier5 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 6", 0f, 100f, "0.##", Order = 5, RequireRestart = false)]
        [SettingPropertyGroup("Companion kill rewards/{PerTierCompanionKillRewardsGroup}")]
        public float CompanionWindsPerKillTier6
        {
            get => _companionWindsPerKillTier6;
            set { if (_companionWindsPerKillTier6 != value) { _companionWindsPerKillTier6 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 7", 0f, 100f, "0.##", Order = 6, RequireRestart = false)]
        [SettingPropertyGroup("Companion kill rewards/{PerTierCompanionKillRewardsGroup}")]
        public float CompanionWindsPerKillTier7
        {
            get => _companionWindsPerKillTier7;
            set { if (_companionWindsPerKillTier7 != value) { _companionWindsPerKillTier7 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 8", 0f, 100f, "0.##", Order = 7, RequireRestart = false)]
        [SettingPropertyGroup("Companion kill rewards/{PerTierCompanionKillRewardsGroup}")]
        public float CompanionWindsPerKillTier8
        {
            get => _companionWindsPerKillTier8;
            set { if (_companionWindsPerKillTier8 != value) { _companionWindsPerKillTier8 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Tier 9", 0f, 100f, "0.##", Order = 8, RequireRestart = false)]
        [SettingPropertyGroup("Companion kill rewards/{PerTierCompanionKillRewardsGroup}")]
        public float CompanionWindsPerKillTier9
        {
            get => _companionWindsPerKillTier9;
            set { if (_companionWindsPerKillTier9 != value) { _companionWindsPerKillTier9 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyBool("Enable companion heal rewards", IsToggle = true, Order = 0, RequireRestart = false, HintText = "Grant winds when a party companion's heal spell ends.")]
        [SettingPropertyGroup("Companion heal rewards", GroupOrder = 7)]
        public bool EnableCompanionHealRewards
        {
            get => _enableCompanionHealRewards;
            set
            {
                if (_enableCompanionHealRewards == value)
                    return;

                _enableCompanionHealRewards = value;
                OnPropertyChanged();
            }
        }

        [SettingPropertyDropdown("Restore target", Order = 1, RequireRestart = false, HintText = "Who receives companion heal winds. Main hero, Self, or Both.")]
        [SettingPropertyGroup("Companion heal rewards")]
        public Dropdown<CompanionWindsRecipientOption> CompanionHealRestoreTarget
        {
            get => _companionHealRestoreTarget;
            set
            {
                if (_companionHealRestoreTarget == value)
                    return;

                _companionHealRestoreTarget = value;
                OnPropertyChanged();
            }
        }

        [SettingPropertyFloatingInteger("Winds per selected amount HP", 0f, 100f, "0.##", Order = 2, RequireRestart = false, HintText = "Winds restored when a heal spell ends. Total payout = (HP healed ÷ HP block for winds) × this value. Set to 0 to disable.")]
        [SettingPropertyGroup("Companion heal rewards")]
        public float CompanionWindsPerHealBlock
        {
            get => _companionWindsPerHealBlock;
            set { if (_companionWindsPerHealBlock != value) { _companionWindsPerHealBlock = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("HP block for winds", 1f, 10000f, "0.##", Order = 3, RequireRestart = false, HintText = "How much HP must be restored for one wind payout. Default 100 with 1.0 winds = 1 wind per 100 HP healed.")]
        [SettingPropertyGroup("Companion heal rewards")]
        public float CompanionHealHpPerWind
        {
            get => _companionHealHpPerWind;
            set { if (_companionHealHpPerWind != value) { _companionHealHpPerWind = value; OnPropertyChanged(); } }
        }

        [SettingPropertyDropdown("Restore target", Order = 0, RequireRestart = false, HintText = "Who receives companion melee, ranged, and spell damage winds. Main hero, Self, or Both.")]
        [SettingPropertyGroup("Companion melee and ranged damage", GroupOrder = 8)]
        public Dropdown<CompanionWindsRecipientOption> CompanionDamageRestoreTarget
        {
            get => _companionDamageRestoreTarget;
            set
            {
                if (_companionDamageRestoreTarget == value)
                    return;

                _companionDamageRestoreTarget = value;
                OnPropertyChanged();
            }
        }

        [SettingPropertyFloatingInteger("Winds per 100 HP dealt", 0f, 100f, "0.##", Order = 1, RequireRestart = false, HintText = "Winds restored per 100 HP of melee or ranged damage dealt. Scales proportionally (50 HP = half this value). Set to 0 to disable.")]
        [SettingPropertyGroup("Companion melee and ranged damage")]
        public float CompanionWindsPerMeleeDamageBlock
        {
            get => _companionWindsPerMeleeDamageBlock;
            set { if (_companionWindsPerMeleeDamageBlock != value) { _companionWindsPerMeleeDamageBlock = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Friendly fire winds per 100 HP", 0f, 100f, "0.##", Order = 2, RequireRestart = false, HintText = "Winds restored per 100 HP of melee or ranged damage dealt to friendly units. Scales proportionally.")]
        [SettingPropertyGroup("Companion melee and ranged damage")]
        public float CompanionWindsPerFriendlyMeleeDamageBlock
        {
            get => _companionWindsPerFriendlyMeleeDamageBlock;
            set { if (_companionWindsPerFriendlyMeleeDamageBlock != value) { _companionWindsPerFriendlyMeleeDamageBlock = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Winds per 100 HP dealt", 0f, 100f, "0.##", Order = 0, RequireRestart = false, HintText = "Winds restored per 100 HP of spell damage dealt. Scales proportionally (50 HP = half this value). Set to 0 to disable.")]
        [SettingPropertyGroup("Companion spell damage", GroupOrder = 9)]
        public float CompanionWindsPerSpellDamageBlock
        {
            get => _companionWindsPerSpellDamageBlock;
            set { if (_companionWindsPerSpellDamageBlock != value) { _companionWindsPerSpellDamageBlock = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Friendly fire winds per 100 HP", 0f, 100f, "0.##", Order = 1, RequireRestart = false, HintText = "Winds restored per 100 HP of spell damage dealt to friendly units. Scales proportionally.")]
        [SettingPropertyGroup("Companion spell damage")]
        public float CompanionWindsPerFriendlySpellDamageBlock
        {
            get => _companionWindsPerFriendlySpellDamageBlock;
            set { if (_companionWindsPerFriendlySpellDamageBlock != value) { _companionWindsPerFriendlySpellDamageBlock = value; OnPropertyChanged(); } }
        }

        [SettingPropertyBool(
            "Warn at battle start",
            Order = 0,
            RequireRestart = false,
            HintText = "Show a one-time on-screen message when compatibility issues are detected. Full report: wom.diagnostics in battle console.")]
        [SettingPropertyGroup("Diagnostics", GroupOrder = 10)]
        public bool ShowBattleDiagnosticsWarning
        {
            get => _showBattleDiagnosticsWarning;
            set
            {
                if (_showBattleDiagnosticsWarning == value)
                    return;

                _showBattleDiagnosticsWarning = value;
                OnPropertyChanged();
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

        private static float GetWindsForDamage(float damageDealt, float hpPerWind, float windsPerBlock)
        {
            if (damageDealt <= 0f || hpPerWind <= 0f || windsPerBlock <= 0f)
                return 0f;

            return damageDealt / hpPerWind * windsPerBlock;
        }

        private void NotifyKillRewardGroupVisibilityChanged()
        {
            OnPropertyChanged(nameof(KillRewardOptionsGroup));
            OnPropertyChanged(nameof(UniformKillRewardGroup));
            OnPropertyChanged(nameof(PerTierKillRewardsGroup));
        }

        private void NotifyAugmentKillRewardGroupVisibilityChanged()
        {
            OnPropertyChanged(nameof(AugmentKillOptionsGroup));
            OnPropertyChanged(nameof(UniformAugmentKillRewardGroup));
            OnPropertyChanged(nameof(PerTierAugmentKillRewardsGroup));
        }

        private void NotifyCompanionKillRewardGroupVisibilityChanged()
        {
            OnPropertyChanged(nameof(CompanionKillRewardOptionsGroup));
            OnPropertyChanged(nameof(UniformCompanionKillRewardGroup));
            OnPropertyChanged(nameof(PerTierCompanionKillRewardsGroup));
        }

    }
}
