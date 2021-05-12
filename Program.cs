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
                PrintMenuItem("Deposit", ConsoleColor.DarkRed);
                PrintMenuItem("Withdraw", ConsoleColor.DarkCyan);
                PrintMenuItem("Buy Stock", ConsoleColor.DarkGray);
                PrintMenuItem("Sell Stock", ConsoleColor.DarkGreen);
                PrintMenuItem("Portfolio", ConsoleColor.DarkYellow);
                PrintMenuItem("Events", ConsoleColor.Yellow);
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
                        System.Console.WriteLine($"Money: {portfolio.GetState().Money:0.##}");
                        if (portfolio.GetState().Shares != null)
                        {
                            foreach (var stockOwned in portfolio.GetState().Shares)
                            {
                                System.Console.WriteLine($"Stock Ticker: {stockOwned.Key} Quantity: {stockOwned.Value.NumberOfShares} Average Price: {stockOwned.Value.Price:0.##} Profit: {portfolio.GetState().Profit:0.##}");
                            }
                        }
                        break;
                    case "B":
                        var amountOfSharesToBuy = GetAmount();
                        var stock = GetStock();
                        var price = GetPrice();
                        portfolio.BuyShares(stock, amountOfSharesToBuy, price);
                        System.Console.WriteLine($"{username} Bought: {stock} Amount: {amountOfSharesToBuy} Price: {price:0.##}");
                        break;
                    case "S":
                        var amountOfSharesToSell = GetAmount();
                        stock = GetStock();
                        price = GetPrice();
                        portfolio.SellShares(stock, amountOfSharesToSell, price);
                        System.Console.WriteLine($"{username} Sold: {stock} Amount: {amountOfSharesToSell} Price: {price:0.##}");
                        break;
                    case "E":
                        var events = await portfolioRepository.GetAllEvents(username);
                        Console.WriteLine($"Events: {username}");
                        foreach (var evnt in events)
                        {
                            switch (evnt)
                            {
                                case SharesSold sharesSold:
                                    System.Console.WriteLine($"[{sharesSold.DateTime}] {sharesSold.Amount} shares SOLD from {sharesSold.Stock} at Price {sharesSold.Price:0.##}");
                                    break;
                                case SharesBought sharesBought:
                                    System.Console.WriteLine($"[{sharesBought.DateTime}] {sharesBought.Amount} shares BOUGHT from {sharesBought.Stock} at Price {sharesBought.Price:0.##}");
                                    break;
                                case DepositMade depositMade:
                                    System.Console.WriteLine($"[{depositMade.DateTime}] {depositMade.Amount} EUR DEPOSIT");
                                    break;
                                case WithdrawalMade withdrawalMade:
                                    System.Console.WriteLine($"[{withdrawalMade.DateTime}] {withdrawalMade.Amount} EUR WITHDRAWAL");
                                    break;
                            }
                        }
                        break;
                }
                await portfolioRepository.Save(portfolio);
                System.Console.WriteLine();
            }
        }

        private static string GetUsername()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            System.Console.Write("Username: ");
            Console.ForegroundColor = ConsoleColor.White;
            return Console.ReadLine()?.ToUpperInvariant();
        }
        private static int GetAmount()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            System.Console.Write("Amount: ");
            Console.ForegroundColor = ConsoleColor.White;
            return Convert.ToInt32(Console.ReadLine()?.ToUpperInvariant());
        }

        private static string GetStock()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            System.Console.Write("Stock Ticker: ");
            Console.ForegroundColor = ConsoleColor.White;
            return Console.ReadLine()?.ToUpperInvariant();
        }

        private static double GetPrice()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            System.Console.Write("Price: ");
            Console.ForegroundColor = ConsoleColor.White;
            return double.Parse(Console.ReadLine()?.ToUpperInvariant());
        }

        private static void PrintMenuItem(string item, ConsoleColor color = ConsoleColor.White)
        {
            var itemShort = item.FirstOrDefault();
            Console.ForegroundColor = color;
            System.Console.Write($"{itemShort}: ");
            Console.ForegroundColor = ConsoleColor.White;
            System.Console.Write($"{item}");
            System.Console.WriteLine();
        }
    }
}
