using System;

namespace API.Events
{
    public class DepositMade : IEvent
    {
        public string EventType { get; } = "DepositMade";
        public string User { get; }
        public int Amount { get; }
        public DateTime DateTime { get; }

        public DepositMade(string user, int amount, DateTime dateTime)
        {
            User = user;
            Amount = amount;
            DateTime = dateTime;
        }
    }
}