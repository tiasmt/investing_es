using System;

namespace API.Events
{
    public class SharesSold : IEvent
    {
        public string EventType { get; } = "SharesSold";
        public string Stock { get; }
        public string User { get; }
        public int Amount { get; }
        public double Price { get; }
        public DateTime DateTime { get; }

        public SharesSold(string user, string stock, int amount, double price, DateTime dateTime)
        {
            User = user;
            Stock = stock;
            Amount = amount;
            DateTime = dateTime;
            Price = price;
        }
    }
}