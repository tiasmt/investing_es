public class ShareTicker
    {
        public string Name {get;set;}
        public int NumberOfShares {get;set;}
        public int Price {get;set;}
        public ShareTicker(string name, int numberOfShares, int price)
        {
            Name = name;
            NumberOfShares = numberOfShares;
            Price = price;
        }
    }
