using System;

namespace API.Events
{
    public class WithdrawalMade : IEvent
    {
        public string EventType { get; } = nameof(WithdrawalMade);
        public string User { get; }
        public int Amount { get; }
        public DateTime DateTime { get; }

        public WithdrawalMade(string user, int amount, DateTime dateTime)
        {
            User = user;
            Amount = amount;
            DateTime = dateTime;
        }
    }
}
