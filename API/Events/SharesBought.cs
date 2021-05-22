using System;

namespace API.Events
{
    public class SharesBought : IEvent
    {
        public string EventType { get; } = "SharesBought";
        public string Stock { get; }
        public string User { get; }
        public double Price { get; }
        public int Amount { get; }
        public DateTime DateTime { get; }

        public SharesBought(string user, string stock, int amount, double price, DateTime dateTime)
        {
            User = user;
            Stock = stock;
            Amount = amount;
            DateTime = dateTime;
            Price = price;
        }
    }
}
