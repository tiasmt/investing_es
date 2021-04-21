using System;

public class SharesBought : IEvent
{
    public string EventType {get;} = "SharesBought";
    public string Stock {get;}
    public string User { get; }
    public int Price { get; }
    public int Amount { get; }
    public DateTime DateTime { get; }

    public SharesBought(string user, string stock ,int amount, int price, DateTime dateTime)
    {
        User = user;
        Stock = stock;
        Amount = amount;
        DateTime = dateTime;
        Price = price;
    }
}
