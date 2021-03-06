using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.ProductCatalog.Model;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace ECommerceProductCatalog
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class ECommerceProductCatalog : StatefulService, IProductCatalogService
    {
        private IProductRepository _serviceFabricProductRepository;

        public ECommerceProductCatalog(StatefulServiceContext context)
            : base(context)
        { }

        public async Task AddProductAsync(Product product)
        {
            await _serviceFabricProductRepository.AddProduct(product);
        }

        public async Task<Product[]> GetAllProductsAsync()
        {
            return (await _serviceFabricProductRepository.GetAllProducts()).ToArray();
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[]
            {
                new ServiceReplicaListener(context => 
                    new FabricTransportServiceRemotingListener(context, this))
            };
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            _serviceFabricProductRepository = new ServiceFabricProductRepository(this.StateManager);
            // fill in sample values
            var product1 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Soap",
                Description = "Hand washing soap",
                Price = 100,
                Availability = 2,
            };

            var product2 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Car",
                Description = "Go from point one to point two",
                Price = 10000,
                Availability = 1,
            };

            await _serviceFabricProductRepository.AddProduct(product1);
            await _serviceFabricProductRepository.AddProduct(product2);

            IEnumerable<Product> all = await _serviceFabricProductRepository.GetAllProducts();
        }
    }
}
