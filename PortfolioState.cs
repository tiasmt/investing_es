using System.Collections.Generic;

public class PortfolioState
{
    public double Money {get;set;}
    public Dictionary<string, ShareTicker> Shares {get;set;}
    public double Profit {get;set;}

}
