using MCM.Common;

namespace WindsOfMagicRestore.Settings
{
    public sealed class CompanionWindsRecipientOption
    {
        public CompanionWindsRecipientMode Mode { get; }
        public string Label { get; }

        private CompanionWindsRecipientOption(CompanionWindsRecipientMode mode, string label)
        {
            Mode = mode;
            Label = label;
        }

        public override string ToString() => Label;

        public static readonly CompanionWindsRecipientOption[] All =
        {
            new(CompanionWindsRecipientMode.MainHero, "Main hero"),
            new(CompanionWindsRecipientMode.Self, "Self"),
            new(CompanionWindsRecipientMode.Both, "Both"),
        };

        public static Dropdown<CompanionWindsRecipientOption> CreateDefaultDropdown()
        {
            return new Dropdown<CompanionWindsRecipientOption>(All, selectedIndex: 0);
        }

        public static CompanionWindsRecipientMode GetMode(Dropdown<CompanionWindsRecipientOption>? dropdown)
        {
            return dropdown?.SelectedValue?.Mode ?? CompanionWindsRecipientMode.MainHero;
        }
    }
}
