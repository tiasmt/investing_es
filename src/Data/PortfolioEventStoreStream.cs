using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using API;
using API.Events;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using investing_es.src.Data;
using Newtonsoft.Json;

namespace Data
{
    public class PortfolioEventStoreStream : IEventStoreRepository
    {
        private const int SnapshotInterval = 5;
        private readonly IEventStoreConnection _connection;

        public static async Task<PortfolioEventStoreStream> Factory()
        {
            var connectionSettings = ConnectionSettings.Create()
                                                        .KeepReconnecting()
                                                        .KeepRetrying()
                                                        .SetHeartbeatTimeout(TimeSpan.FromMinutes(5))
                                                        .SetHeartbeatInterval(TimeSpan.FromMinutes(1))
                                                        .SetDefaultUserCredentials(new UserCredentials("admin", "changeit"))
                                                        .Build();

            var conn = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
            await conn.ConnectAsync();

            return new PortfolioEventStoreStream(conn);
        }

        private PortfolioEventStoreStream(IEventStoreConnection connection)
        {
            _connection = connection;
        }

        private string GetStreamName(string username)
        {
            return "User-" + username;
        }

        private string GetSnapshotStreamName(string username)
        {
            return "User-Snapshot-" + username;
        }

        private IEvent DeserializeEvent(ResolvedEvent evnt)
        {
            IEvent result = null;

            var esJsonData = Encoding.UTF8.GetString(evnt.Event.Data);
            if (evnt.Event.EventType == nameof(DepositMade))
            {
                result = JsonConvert.DeserializeObject<DepositMade>(esJsonData);
            }
            else if (evnt.Event.EventType == nameof(WithdrawalMade))
            {
                result = JsonConvert.DeserializeObject<WithdrawalMade>(esJsonData);
            }
            else if (evnt.Event.EventType == nameof(SharesBought))
            {
                result = JsonConvert.DeserializeObject<SharesBought>(esJsonData);
            }
            else if (evnt.Event.EventType == nameof(SharesSold))
            {
                result = JsonConvert.DeserializeObject<SharesSold>(esJsonData);
            }

            return result;

        }

        public async Task Save(Portfolio portfolio)
        {

            var streamName = GetStreamName(portfolio.Username);
            var newEvents = portfolio.GetUncommittedEvents();

            long version = 0;

            foreach (var evnt in newEvents)
            {
                var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(evnt));
                var metadata = Encoding.UTF8.GetBytes("{}");
                var evt = new EventData(Guid.NewGuid(), evnt.EventType, true, data, metadata);
                var result = await _connection.AppendToStreamAsync(streamName, ExpectedVersion.Any, evt);
                version = result.NextExpectedVersion;

                if ((version) % SnapshotInterval == 0)
                {
                    await AppendSnapshot(portfolio, version);
                }
            }

        }

        public async Task<Portfolio> Get(string username)
        {
            var streamName = GetStreamName(username);
            var snapshot = await GetSnapshot(username);
            var portfolio = new Portfolio(username, snapshot.State);

            StreamEventsSlice currentSlice;
            var nextSliceStart = snapshot.Version + 1;
            do
            {
                currentSlice = await _connection.ReadStreamEventsForwardAsync(
                    streamName,
                    nextSliceStart,
                    200,
                    false
                );

                nextSliceStart = currentSlice.NextEventNumber;

                foreach (var evnt in currentSlice.Events)
                {
                    var evntObj = DeserializeEvent(evnt);
                    portfolio.ApplyEvent(evntObj, true);
                }
            } while (!currentSlice.IsEndOfStream);
            return portfolio;
        }

        public async Task<List<IEvent>> GetAllEvents(string username)
        {
            var streamName = GetStreamName(username);
            var snapshot = new Snapshot();
            var portfolio = new Portfolio(username, snapshot.State);
            var events = new List<IEvent>();

            StreamEventsSlice currentSlice;
            var nextSliceStart = snapshot.Version;
            do
            {
                currentSlice = await _connection.ReadStreamEventsForwardAsync(
                    streamName,
                    nextSliceStart,
                    200,
                    false
                );

                nextSliceStart = currentSlice.NextEventNumber;

                foreach (var evnt in currentSlice.Events)
                {
                    var evntObj = DeserializeEvent(evnt);
                    events.Add(evntObj);
                }
            } while (!currentSlice.IsEndOfStream);
            return events;
        }

        private async Task<Snapshot> GetSnapshot(string sku)
        {
            var streamName = GetSnapshotStreamName(sku);
            var slice = await _connection.ReadStreamEventsBackwardAsync(streamName, StreamPosition.End, 1, false);
            if (slice.Events.Any())
            {
                var evnt = slice.Events.First();
                var json = Encoding.UTF8.GetString(evnt.Event.Data);
                return JsonConvert.DeserializeObject<Snapshot>(json);
            }

            return new Snapshot();
        }

        private async Task AppendSnapshot(Portfolio portfolio, long version)
        {
            var streamName = GetSnapshotStreamName(portfolio.Username);
            var state = portfolio.GetState();

            var snapshot = new Snapshot
            {
                State = state,
                Version = version
            };

            var metadata = Encoding.UTF8.GetBytes("{}");
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(snapshot));
            var evt = new EventData(Guid.NewGuid(), "snapshot", true, data, metadata);
            await _connection.AppendToStreamAsync(streamName, ExpectedVersion.Any, evt);
        }
    }
}