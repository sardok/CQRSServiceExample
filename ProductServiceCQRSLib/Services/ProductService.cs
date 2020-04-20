using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using SimpleMessageQueue;
using ProductServiceCQRSLib.DataContexts.Command;
using ProductServiceCQRSLib.Services.Command;
using ProductServiceCQRSLib.Services.Query;

namespace ProductServiceCQRSLib
{
    public class ProductService
    {
        readonly CommandService commandService;
        readonly QueryService queryService;
        readonly IMessageQueue messageQueue;

        public ProductService(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CommandServiceDataContext>()
                .UseSqlite(connectionString);
            messageQueue = new MessageQueue();
            commandService = new CommandService(optionsBuilder.Options, messageQueue);
            queryService = new QueryService(messageQueue);
        }

        public async Task AddAmount(string productId, int amount)
        {
            ThrowIfInvalidProductId(productId);
            ThrowIfInvalidAddAmount(amount);

            var product = queryService.GetProduct(productId);
            var newAmount = product.Amount + amount;
            await commandService.UpdateAmount(productId, newAmount);
        }

        public async Task SubAmount(string productId, int amount)
        {
            ThrowIfInvalidProductId(productId);
            ThrowIfInvalidAddAmount(amount);

            var product = queryService.GetProduct(productId);
            var newAmount = product.Amount - amount;
            ThrowIfInvalidSetAmount(newAmount);
            await commandService.UpdateAmount(productId, newAmount);
        }

        public async Task SetAmount(string productId, int amount)
        {
            ThrowIfInvalidProductId(productId);
            ThrowIfInvalidSetAmount(amount);

            await commandService.UpdateAmount(productId, amount);
        }

        public Product GetProduct(string productId)
        {
            ThrowIfInvalidProductId(productId);

            return queryService.GetProduct(productId);
        }

        public async Task<string> Create(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            ThrowIfInvalidSetAmount(product.Amount);

            var productId = await commandService.Create(product.Title, product.Amount);
            return productId;
        }

        public async Task Delete(string productId)
        {
            ThrowIfInvalidProductId(productId);

            await commandService.Delete(productId);
        }

        public async Task UpdateTitle(string productId, string title)
        {
            ThrowIfInvalidProductId(productId);

            await commandService.UpdateTitle(productId, title);
        }

        void ThrowIfInvalidProductId(string productId)
        {
            if (string.IsNullOrEmpty(productId))
                throw new ArgumentNullException(nameof(productId));
        }

        void ThrowIfInvalidAddAmount(int amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than 0", nameof(amount));
        }

        void ThrowIfInvalidSetAmount(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Cannot set amount to negative value");
        }
    }
}