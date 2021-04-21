using System;
using System.Collections.Generic;
using System.Linq;

public class Portfolio
    {
        public string Username { get; }
        private readonly IList<IEvent> _events = new List<IEvent>();
        private readonly IList<IEvent> _uncommittedevents = new List<IEvent>();
        public int Version { get; protected set; }
        //Projection (Current State)
        private readonly PortfolioState _portfolioState;

        public PortfolioState GetState()
        {
            return _portfolioState;
        }

        public Portfolio(string username, PortfolioState state)
        {
            Username = username;
            _portfolioState = state;
        }

        public void Deposit(int quantity)
        {
            ApplyEvent(new DepositMade(Username, quantity, DateTime.UtcNow));
        }

        public void Withdrawal(int quantity)
        {
            ApplyEvent(new WithdrawalMade(Username, quantity, DateTime.UtcNow));
        }

        public void BuyShares(string stock, int quantity, int price)
        {
            ApplyEvent(new SharesBought(Username, stock, quantity, price, DateTime.UtcNow));
        }
        public void SellShares(string stock, int quantity, int price)
        {
            ApplyEvent(new SharesSold(Username, stock, quantity, price, DateTime.UtcNow));
        }

        private void Apply(SharesSold evnt)
        {
            _portfolioState.Money += (evnt.Amount * evnt.Price);
            if(_portfolioState.Shares == null) _portfolioState.Shares = new Dictionary<string, int>(); 
            if(_portfolioState.Shares.ContainsKey(evnt.Stock))
            {
                _portfolioState.Shares[evnt.Stock] -= evnt.Amount;
            } 
            else
            {
                //do nothing
            }
        }
        private void Apply(SharesBought evnt)
        {
            _portfolioState.Money -= (evnt.Amount * evnt.Price);
            
            if(_portfolioState.Shares == null) _portfolioState.Shares = new Dictionary<string, int>(); 
            if(_portfolioState.Shares.ContainsKey(evnt.Stock))
            {
                _portfolioState.Shares[evnt.Stock] += evnt.Amount;
            } 
            else
            {
                _portfolioState.Shares.Add(evnt.Stock, evnt.Amount);
            }

        }
        private void Apply(DepositMade evnt)
        {
            _portfolioState.Money += evnt.Amount;
        }
        private void Apply(WithdrawalMade evnt)
        {
            _portfolioState.Money -= evnt.Amount;
        }

        public IList<IEvent> GetEvents()
        {
            return _events;
        }

        public IList<IEvent> GetUncommittedEvents()
        {
            //todo
            var uncommittedEvents = _uncommittedevents.ToArray();
            _uncommittedevents.Clear();
            return uncommittedEvents;
        }

       public void ApplyEvent(IEvent evnt, bool isFastForward = false)
        {
            switch (evnt)
            {
                case SharesBought sharesBought:
                    Apply(sharesBought);
                    break;
                case SharesSold sharesSold:
                    Apply(sharesSold);
                    break;
                case DepositMade depositMade:
                    Apply(depositMade);
                    break;
                case WithdrawalMade withdrawalMade:
                    Apply(withdrawalMade);
                    break;
            }

            
            if(isFastForward == false) 
                _uncommittedevents.Add(evnt);
            else 
                _events.Add(evnt);
        }
    }
