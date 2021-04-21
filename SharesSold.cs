using System;

public class SharesSold : IEvent
{
    public string EventType {get;} = "SharesSold";
    public string Stock {get;}
    public string User { get; }
    public int Amount { get; }
    public int Price { get; }
    public DateTime DateTime { get; }

    public SharesSold(string user, string stock ,int amount, int price, DateTime dateTime)
    {
        User = user;
        Stock = stock;
        Amount = amount;
        DateTime = dateTime;
        Price = price;
    }
}