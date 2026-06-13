using System;
using System.Globalization;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace WindsOfMagicRestore.Infrastructure
{
    internal static class WindsRestoreMessages
    {
        private const float FlushIntervalSeconds = 5f;
        private const float MinDisplayAmount = 0.01f;
        private static readonly Color MessageColor = new Color(0.35f, 0.65f, 1f);

        private static float _pendingAmount;
        private static float _nextFlushTime;

        public static void Reset()
        {
            _pendingAmount = 0f;
            _nextFlushTime = 0f;
        }

        public static void Record(float amount)
        {
            if (amount <= 0f || Mission.Current == null)
                return;

            _pendingAmount += amount;
            TryFlush(Mission.Current.CurrentTime);
        }

        public static void Tick(float missionTime)
        {
            TryFlush(missionTime);
        }

        public static void FlushRemaining(float missionTime)
        {
            if (_pendingAmount < MinDisplayAmount)
                return;

            DisplayPending(missionTime);
        }

        private static void TryFlush(float missionTime)
        {
            if (_pendingAmount < MinDisplayAmount || missionTime < _nextFlushTime)
                return;

            DisplayPending(missionTime);
        }

        private static void DisplayPending(float missionTime)
        {
            var amount = _pendingAmount;
            _pendingAmount = 0f;
            _nextFlushTime = missionTime + FlushIntervalSeconds;

            var amountText = amount.ToString("0.##", CultureInfo.InvariantCulture);
            var unit = Math.Abs(amount - 1f) < 0.005f ? "Wind" : "Winds";
            var text = $"+{amountText} {unit} of Magic restored";

            InformationManager.DisplayMessage(new InformationMessage(text, MessageColor));
        }
    }
}
