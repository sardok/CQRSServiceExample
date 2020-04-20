using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using ProductApi.Controllers;
using ProductServiceCQRSLib;
using Shared.Models;

namespace ProductApi.Tests
{
    public abstract class ProductApiTestBase : IDisposable
    {
        protected readonly ProductController productController;
        Product productUnderTest;
        readonly string dbPath;
        bool disposed = false;

        public ProductApiTestBase()
        {
            dbPath = Path.GetTempFileName();
            var productService = new ProductService($"Data source={dbPath};", true);
            productController = new ProductController(productService);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                File.Delete(dbPath);
            }

            disposed = true;
        }

        protected async Task<string> GivenProductToBeCreated(string title, int amount)
        {
            var product = new Product
            {
                Title = title,
                Amount = amount,
            };
            var productId = await productController.Create(product);
            return productId;
        }

        protected void WhenGetRequested(string productId)
        {
            productUnderTest = productController.GetProduct(productId);
        }

        protected async Task WhenDeleteRequested(string productId)
        {
            await productController.Delete(productId);
        }

        protected async Task WhenTitleUpdateRequested(string productId, string title)
        {
            var product = new Product
            {
                Id = productId,
                Title = title,
            };
            await productController.UpdateTitle(product);
        }

        protected async Task WhenAddAmountRequested(string productId, int amount)
        {
            await productController.AddAmount(productId, amount);
        }

        protected async Task WhenSubAmountRequested(string productId, int amount)
        {
            await productController.SubAmount(productId, amount);
        }

        protected void ThenProductMustMatch(Product expected)
        {
            ThenProductMustMatch(expected.Id, expected.Title, expected.Amount);
        }

        protected void ThenProductMustMatch(string id, string title, int amount)
        {
            WhenGetRequested(id);
            Assert.Equal(id, productUnderTest.Id);
            Assert.Equal(title, productUnderTest.Title);
            Assert.Equal(amount, productUnderTest.Amount);
        }

        protected void ThenProductMustBeNull(string productId)
        {
            WhenGetRequested(productId);
            Assert.Null(productUnderTest);
        }
    }
}
