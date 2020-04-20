using System.Threading.Tasks;
using Shared.Models;
using Microsoft.AspNetCore.Mvc;
using ProductServiceCQRSLib;

namespace ProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        readonly ProductService productService;

        public ProductController(ProductService productService)
        {
            this.productService = productService;
        }

        [HttpPost("update-title")]
        public async Task UpdateTitle([FromBody] Product product)
        {
            await productService.UpdateTitle(product.Id, product.Title);
        }

        [HttpPost("{id}/add-amount/{amount:int}")]
        public async Task AddAmount(string id, int amount)
        {
            await productService.AddAmount(id, amount);
        }

        [HttpPost("{id}/sub-amount/{amount:int}")]
        public async Task SubAmount(string id, int amount)
        {
            await productService.SubAmount(id, amount);
        }

        [HttpPost("{id}/set-amount/{amount:int}")]
        public async Task SetAmount(string id, int amount)
        {
            await productService.SetAmount(id, amount);
        }

        [HttpGet("{id}")]
        public Product GetProduct(string id)
        {
            return productService.GetProduct(id);
        }

        [HttpPost]
        public async Task<string> Create([FromBody] Product product)
        {
            return await productService.Create(product);
        }

        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            await productService.Delete(id);
        }
    }
}