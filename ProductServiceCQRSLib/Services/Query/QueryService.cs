using System.Collections.Generic;
using SimpleMessageQueue;
using Shared.Models;
using ProductServiceCQRSLib.Models;

namespace ProductServiceCQRSLib.Services.Query
{
    public class QueryService
    {
        readonly IMessageQueue messageQueue;
        int seq;
        readonly object lock_ = new object();
        readonly Dictionary<string, Product> data = 
            new Dictionary<string, Product>();

        public QueryService(IMessageQueue messageQueue)
        {
            this.messageQueue = messageQueue;
            this.messageQueue.Subscribe<ProductCreated>(OnProductCreated);
            this.messageQueue.Subscribe<ProductDeleted>(OnProductDeleted);
            this.messageQueue.Subscribe<AmountUpdated>(OnAmountUpdated);
            this.messageQueue.Subscribe<TitleUpdated>(OnTitleUpdated);
        }

        public Product GetProduct(string productId)
        {
            lock (lock_)
            {
                if (data.TryGetValue(productId, out var product))
                {
                    return product.Clone();
                }

                return null;
            }
        }

        void OnProductCreated(ProductCreated productCreated)
        {
            lock (lock_)
            {
                if (CheckMessageSanity(productCreated))
                {
                    var product = productCreated.Product;
                    data[product.Id] = product;
                }
            }
        }

        void OnProductDeleted(ProductDeleted productDeleted)
        {
            lock (lock_)
            {
                if (CheckMessageSanity(productDeleted))
                {
                    data.Remove(productDeleted.ProductId);
                }
            }
        }

        void OnAmountUpdated(AmountUpdated amountUpdated)
        {
            lock (lock_)
            {
                if (CheckMessageSanity(amountUpdated))
                {
                    if (data.ContainsKey(amountUpdated.ProductId))
                    {
                        var entry = data[amountUpdated.ProductId];
                        entry.Amount = amountUpdated.Amount;
                    }
                }
            }
        }

        void OnTitleUpdated(TitleUpdated titleUpdated)
        {
            lock (lock_)
            {
                if (CheckMessageSanity(titleUpdated))
                {
                    if (data.ContainsKey(titleUpdated.ProductId))
                    {
                        var entry = data[titleUpdated.ProductId];
                        entry.Title = titleUpdated.Title;
                    }
                }
            }
        }

        bool CheckMessageSanity(CommandBase commandBase)
        {
            if (CheckAndIncSeq(commandBase.Seq))
            {
                return true;
            }
            else
            {
                RequestReplay();
                return false;
            }
        }

        bool CheckAndIncSeq(long messageSeq)
        {
            if (seq + 1 != messageSeq)
                return false;

            seq += 1;
            return true;
        }

        void RequestReplay()
        {
            var replayEvents = new ReplayEvents(seq + 1);
            messageQueue.Publish(replayEvents);
        }
    }
}
