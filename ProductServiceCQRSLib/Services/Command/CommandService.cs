using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimpleMessageQueue;
using Shared.Models;
using ProductServiceCQRSLib.Models;
using ProductServiceCQRSLib.Models.Command;
using ProductServiceCQRSLib.DataContexts.Command;

namespace ProductServiceCQRSLib.Services.Command
{
    public class CommandService
    {
        struct EventTypes
        {
            public const string AmountUpdated = "AmountUpdated";
            public const string TitleUpdated = "TitleUpdated";
            public const string ProductCreated = "ProductCreated";
            public const string ProductDeleted = "ProductDeleted";
        }

        readonly IMessageQueue messageQueue;
        readonly DbContextOptions<CommandServiceDataContext> dbContextOptions;
        long seq = 0;

        public CommandService(
            DbContextOptions<CommandServiceDataContext> dbContextOptions,
            IMessageQueue messageQueue)
        {
            this.dbContextOptions = dbContextOptions;
            this.messageQueue = messageQueue;
            this.messageQueue.Subscribe<ReplayEvents>(OnReplayEvents);
            EnsureDbCreated();
        }

        void EnsureDbCreated()
        {
            using (var dbContext = new CommandServiceDataContext(dbContextOptions))
            {
                dbContext.Database.EnsureCreated();
            }
        }

        public async Task UpdateAmount(string id, int amount)
        {
            var @event = await CreateEvent(id, EventTypes.AmountUpdated, amount);
            var amountUpdated = AmountUpdated.Create(@event, id, amount);
            messageQueue.Publish(amountUpdated);
        }

        public async Task UpdateTitle(string id, string title)
        {
            var @event = await CreateEvent(id, EventTypes.TitleUpdated, title);
            var titleUpdated = TitleUpdated.Create(@event, id, title);
            messageQueue.Publish(titleUpdated);
        }

        public async Task<string> Create(string title, int amount)
        {
            var product = new Product
            {
                Id = Guid.NewGuid().ToString(),
                Title = title,
                Amount = amount,
            };
            var @event = await CreateEvent(
                product.Id, EventTypes.ProductCreated, product);
            var productCreated = ProductCreated.Create(@event, product);
            messageQueue.Publish(productCreated);
            return product.Id;
        }

        public async Task Delete(string productId)
        {
            var @event = await CreateEvent<object>(
                productId, EventTypes.ProductDeleted, null);
            var productDeleted = ProductDeleted.Create(@event, productId);
            messageQueue.Publish(productDeleted);
        }

        void OnReplayEvents(ReplayEvents replayEvents)
        {
            using (var dataContext = new CommandServiceDataContext(dbContextOptions))
            {
                var query = dataContext.Events
                    .Where(evt => evt.Seq >= replayEvents.From)
                    .AsQueryable();
                if (replayEvents.To.HasValue)
                {
                    query = query.Where(evt => evt.Seq <= replayEvents.To);
                }

                var events = query
                    .OrderBy(evt => evt.Seq)
                    .ToArray();

                foreach (var @event in events)
                {
                    var message = ConstructMessage<IMessage<object>>(@event);
                    messageQueue.Publish(message);
                }
            }
        }

        IMessage<object> ConstructMessage<T>(Event @event)
        {
            var data = @event.Data;
            if (data == null)
                throw new ArgumentNullException(nameof(@event.Data));

            switch(data.Type)
            {
                case EventTypes.AmountUpdated:
                    var amount = data.Payload as int?;
                    if (amount != null)
                    {
                        return (IMessage<object>) AmountUpdated.Create(@event, data.Id, amount.Value);
                    }
                    break;
                case EventTypes.ProductCreated:
                    if (data.Payload is Product product)
                    {
                        return (IMessage<object>)ProductCreated.Create(@event, product);
                    }
                    break;
                case EventTypes.ProductDeleted:
                    return (IMessage<object>) ProductDeleted.Create(@event, data.Id);
                case EventTypes.TitleUpdated:
                    var title = data.Payload as string;
                    return (IMessage<object>) TitleUpdated.Create(@event, data.Id, title);
                default:
                    break;
            }

            throw new ArgumentException($"InvalidEvent data: {data.Id}, " +
                $"{data.Type}, {data.Payload}");
        }

        async Task<Event> CreateEvent<T>(string productId, string eventType, T arg)
        {
            using (var dataContext = new CommandServiceDataContext(dbContextOptions))
            {
                var @event = new Event
                {
                    Seq = Interlocked.Increment(ref seq),
                    Data = MakeEventData(productId, eventType, arg),
                };
                await dataContext.Events.AddAsync(@event);
                await dataContext.SaveChangesAsync();

                return @event;
            }
        }

        EventData MakeEventData(string id, string eventType, object payload)
        {
            return new EventData
            {
                Id = id,
                Type = eventType,
                Payload = payload,
            };
        }
    }
}