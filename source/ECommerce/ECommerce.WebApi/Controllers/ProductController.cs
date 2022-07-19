using Ecommerce.ProductCatalog.Model;
using ECommerce.WebApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;

namespace ECommerce.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductCatalogService _productCatalogService;
        public ProductsController(ILogger<ProductsController> logger)
        {
            _logger = logger;

            var proxyFactory = new ServiceProxyFactory(c => new FabricTransportServiceRemotingClientFactory());
            // URI = fabric:ApplicationName/ServiceName
            _productCatalogService = proxyFactory.CreateServiceProxy<IProductCatalogService>(
                new Uri("fabric:/ECommerce/ECommerceProductCatalog"),
                new ServicePartitionKey(0));
        }

        [HttpGet]
        public async Task<IEnumerable<ApiProduct>> GetAsync()
        {
            IEnumerable<Product> allProducts = await _productCatalogService.GetAllProductsAsync();
            return allProducts.Select(p => new ApiProduct
            {
                Id = p.Id,
                Description = p.Description
            });
        }
        
        [HttpPost]
        public async Task PostAsync([FromBody] ApiProduct product)
        {
            var newProduct = new Product()
            {
                Id = product.Id,
                Description = product.Description
            };

            await _productCatalogService.AddProductAsync(newProduct);
        }
    }
}