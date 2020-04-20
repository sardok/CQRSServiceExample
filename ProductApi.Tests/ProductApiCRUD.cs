using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Shared.Models;

namespace ProductApi.Tests
{
    public class ProductApiCRUD : ProductApiTestBase
    {
        [Fact]
        public async Task TestCreate()
        {
            var productId = await GivenProductToBeCreated("Beer Kit", 10);
            ThenProductMustMatch(productId, "Beer Kit", 10);
        }

        [Fact]
        public async Task TestDelete()
        {
            var productId = await GivenProductToBeCreated("Short life span", 0);
            await WhenDeleteRequested(productId);
            ThenProductMustBeNull(productId);
        }

        [Fact]
        public async Task TestTitleUpdate()
        {
            var productId = await GivenProductToBeCreated("Item 1", 0);
            await WhenTitleUpdateRequested(productId, "Item 1 Updated");
            ThenProductMustMatch(productId, "Item 1 Updated", 0);
        }

        [Fact]
        public async Task TestAmountUpdate()
        {
            var productId = await GivenProductToBeCreated("Item 2", 5);
            await WhenAddAmountRequested(productId, 5);
            ThenProductMustMatch(productId, "Item 2", 10);

            await WhenSubAmountRequested(productId, 7);
            ThenProductMustMatch(productId, "Item 2", 3);
        }

        [Fact]
        public async Task TestAmountBounds()
        {
            var productId = await GivenProductToBeCreated("Item 3", 2);
            await Assert.ThrowsAsync<ArgumentException>(
                () => WhenSubAmountRequested(productId, 3));
        }
    }
}
