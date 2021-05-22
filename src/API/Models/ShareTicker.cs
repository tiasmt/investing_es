
namespace API.Models
{
    public class ShareTicker
    {
        public int NumberOfShares { get; set; }
        public double Price { get; set; }
        public ShareTicker(int numberOfShares, double price)
        {
            NumberOfShares = numberOfShares;
            Price = price;
        }
    }
}
