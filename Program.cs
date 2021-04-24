using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace investing_es
{
    class Program
    {
        public static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {

            var portfolioRepository = await PortfolioEventStoreStream.Factory();

            var key = string.Empty;
            while (key != "X")
            {
                System.Console.WriteLine("D: Deposit");
                System.Console.WriteLine("W: Withdraw");
                System.Console.WriteLine("B: Buy Stock");
                System.Console.WriteLine("S: Sell Stock");
                System.Console.WriteLine("P: Portfolio");
                System.Console.WriteLine("E: Events");
                System.Console.WriteLine(">  ");
                key = Console.ReadLine()?.ToUpperInvariant();
                System.Console.WriteLine();

                var username = GetUsername();
                var portfolio = await portfolioRepository.Get(username);

                switch (key)
                {
                    case "D":
                        var moneyToDeposit = GetAmount();
                        portfolio.Deposit(moneyToDeposit);
                        System.Console.WriteLine($"{username} Received: {moneyToDeposit}");
                        break;
                    case "W":
                        var moneyToWithdraw = GetAmount();
                        portfolio.Withdrawal(moneyToWithdraw);
                        System.Console.WriteLine($"{username} Withdrew: {moneyToWithdraw}");
                        break;
                    case "P":
                        System.Console.WriteLine($"{username} Money: {portfolio.GetState().Money}");
                        if(portfolio.GetState().Shares != null) 
                        {
                            foreach(var stockOwned in portfolio.GetState().Shares)
                            {
                                System.Console.WriteLine($"{username} Stock Ticker: {stockOwned.Key} Quantity: {stockOwned.Value.NumberOfShares} Average Price: {stockOwned.Value.Price} Profit: {portfolio.GetState().Profit}");
                            }
                        }
                        break;
                    case "B":
                        var amountOfSharesToBuy = GetAmount();
                        var stock = GetStock();
                        var price = GetPrice();
                        portfolio.BuyShares(stock, amountOfSharesToBuy, price);
                        System.Console.WriteLine($"{username} Bought: {stock} Amount: {amountOfSharesToBuy} Price: {price}");
                        break;
                    case "S":
                        var amountOfSharesToSell = GetAmount();
                        stock = GetStock();
                        price = GetPrice();
                        portfolio.SellShares(stock, amountOfSharesToSell, price);
                        System.Console.WriteLine($"{username} Sold: {stock} Amount: {amountOfSharesToSell} Price: {price}");
                        break;
                    case "E":
                    //todo - fix how events are retrieved
                        /*Console.WriteLine($"Events: {username}");
                        foreach (var evnt in portfolio.GetEvents())
                        {
                            switch (evnt)
                            {
                                case ProductShipped shipProduct:
                                    System.Console.WriteLine($"{shipProduct.DateTime:u} {sku} Shipped: {shipProduct.Quantity}");
                                    break;
                                case ProductReceived receiveProduct:
                                    System.Console.WriteLine($"{receiveProduct.DateTime:u} {sku} Received: {receiveProduct.Quantity}");
                                    break;
                                case InventoryAdjusted inventoryAdjusted:
                                    System.Console.WriteLine($"{inventoryAdjusted.DateTime:u} {sku} Adjused: {inventoryAdjusted.Quantity} Reason: {inventoryAdjusted.Reason}");
                                    break;
                            }
                        }*/
                        break;
                }
                await portfolioRepository.Save(portfolio);
                System.Console.WriteLine();
            }
        }

        private static string GetUsername()
        {
            System.Console.WriteLine("Username: ");
            return Console.ReadLine()?.ToUpperInvariant();
        }
        private static int GetAmount()
        {
            System.Console.WriteLine("Amount: ");
            return Convert.ToInt32(Console.ReadLine()?.ToUpperInvariant());
        }

        private static string GetStock()
        {
            System.Console.WriteLine("Stock Ticker: ");
            return Console.ReadLine()?.ToUpperInvariant();
        }

        private static double GetPrice()
        {
            System.Console.WriteLine("Price: ");
            return double.Parse(Console.ReadLine()?.ToUpperInvariant());
        }
    }
}
