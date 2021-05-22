using System.Collections.Generic;
using System.Threading.Tasks;
using API;
using API.Events;

namespace investing_es.src.Data
{
    public interface IEventStoreRepository
    {
         Task<Portfolio> Get(string username);
         Task<List<IEvent>> GetAllEvents(string username);
    }
}