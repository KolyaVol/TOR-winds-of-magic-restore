using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace WindsOfMagicRestore.Settings
{
    public sealed class WindsOfMagicRestoreSettings : AttributeGlobalSettings<WindsOfMagicRestoreSettings>
    {
        private float _windsPerKillTier1 = 1f;
        private float _windsPerKillTier2;
        private float _windsPerKillTier3;
        private float _windsPerKillTier4;
        private float _windsPerKillTier5;
        private float _windsPerKillTier6;
        private float _windsPerHealBlock = 1f;
        private float _healHpPerWind = 100f;
        private float _windsPerSecond = 0.05f;
        private float _windsOnDamageDealt;
        private float _windsPerCampaignTick;

        public override string Id => "WindsOfMagicRestore_v1";
        public override string DisplayName => "Winds of Magic Restore";
        public override string FolderName => "WindsOfMagicRestore";
        public override string FormatType => "json";

        public static void Initialize()
        {
            _ = Instance;
        }

        [SettingPropertyFloatingInteger("Winds per kill (tier 1)", 0f, 100f, "0.##", Order = 0, RequireRestart = false, HintText = "Winds restored when killing a tier 1 enemy.")]
        [SettingPropertyGroup("Kill rewards", GroupOrder = 0)]
        public float WindsPerKillTier1
        {
            get => _windsPerKillTier1;
            set { if (_windsPerKillTier1 != value) { _windsPerKillTier1 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Winds per kill (tier 2)", 0f, 100f, "0.##", Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards")]
        public float WindsPerKillTier2
        {
            get => _windsPerKillTier2;
            set { if (_windsPerKillTier2 != value) { _windsPerKillTier2 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Winds per kill (tier 3)", 0f, 100f, "0.##", Order = 2, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards")]
        public float WindsPerKillTier3
        {
            get => _windsPerKillTier3;
            set { if (_windsPerKillTier3 != value) { _windsPerKillTier3 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Winds per kill (tier 4)", 0f, 100f, "0.##", Order = 3, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards")]
        public float WindsPerKillTier4
        {
            get => _windsPerKillTier4;
            set { if (_windsPerKillTier4 != value) { _windsPerKillTier4 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Winds per kill (tier 5)", 0f, 100f, "0.##", Order = 4, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards")]
        public float WindsPerKillTier5
        {
            get => _windsPerKillTier5;
            set { if (_windsPerKillTier5 != value) { _windsPerKillTier5 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Winds per kill (tier 6)", 0f, 100f, "0.##", Order = 5, RequireRestart = false)]
        [SettingPropertyGroup("Kill rewards")]
        public float WindsPerKillTier6
        {
            get => _windsPerKillTier6;
            set { if (_windsPerKillTier6 != value) { _windsPerKillTier6 = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Winds per heal block", 0f, 100f, "0.##", Order = 0, RequireRestart = false, HintText = "Winds granted when a heal spell ends. Default: 1 wind per block.")]
        [SettingPropertyGroup("Heal rewards", GroupOrder = 1)]
        public float WindsPerHealBlock
        {
            get => _windsPerHealBlock;
            set { if (_windsPerHealBlock != value) { _windsPerHealBlock = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("HP healed per block", 1f, 10000f, "0.##", Order = 1, RequireRestart = false, HintText = "How much healing must be done in one spell session for one block. Default: 100 HP = 1 wind (with default block reward).")]
        [SettingPropertyGroup("Heal rewards")]
        public float HealHpPerWind
        {
            get => _healHpPerWind;
            set { if (_healHpPerWind != value) { _healHpPerWind = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Winds per second", 0f, 10f, "0.####", Order = 0, RequireRestart = false, HintText = "Passive in-battle Winds of Magic regen. Independent of healing.")]
        [SettingPropertyGroup("Passive regen", GroupOrder = 2)]
        public float WindsPerSecond
        {
            get => _windsPerSecond;
            set { if (_windsPerSecond != value) { _windsPerSecond = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Winds on damage dealt", 0f, 100f, "0.##", Order = 0, RequireRestart = false, HintText = "Reserved for future use. Not active yet.")]
        [SettingPropertyGroup("Future", GroupOrder = 3)]
        public float WindsOnDamageDealt
        {
            get => _windsOnDamageDealt;
            set { if (_windsOnDamageDealt != value) { _windsOnDamageDealt = value; OnPropertyChanged(); } }
        }

        [SettingPropertyFloatingInteger("Winds per campaign tick", 0f, 100f, "0.##", Order = 1, RequireRestart = false, HintText = "Reserved for future use. Not active yet.")]
        [SettingPropertyGroup("Future")]
        public float WindsPerCampaignTick
        {
            get => _windsPerCampaignTick;
            set { if (_windsPerCampaignTick != value) { _windsPerCampaignTick = value; OnPropertyChanged(); } }
        }

        public float GetWindsForTier(int tier)
        {
            return tier switch
            {
                1 => WindsPerKillTier1,
                2 => WindsPerKillTier2,
                3 => WindsPerKillTier3,
                4 => WindsPerKillTier4,
                5 => WindsPerKillTier5,
                6 => WindsPerKillTier6,
                _ => WindsPerKillTier1,
            };
        }

        public float GetWindsForHealing(int totalHpHealed)
        {
            if (totalHpHealed <= 0 || HealHpPerWind <= 0f || WindsPerHealBlock <= 0f)
                return 0f;

            return totalHpHealed / HealHpPerWind * WindsPerHealBlock;
        }
    }
}
