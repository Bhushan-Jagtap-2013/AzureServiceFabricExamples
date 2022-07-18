using ECommerce.WebApi.Model;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ILogger<ProductsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<ApiProduct> Get()
        {
            return new[] { new ApiProduct { Id = Guid.NewGuid()} };
        }
        
        [HttpPost]
        public async Task PostAsync([FromBody] ApiProduct product)
        {

        }
    }
}