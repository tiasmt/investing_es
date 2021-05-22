using API.Models;

namespace Data
{
    public class Snapshot
    {
        public long Version { get; set; } = 0;
        public PortfolioState State;
        public Snapshot()
        {
            State = new PortfolioState();
        }
    }
}