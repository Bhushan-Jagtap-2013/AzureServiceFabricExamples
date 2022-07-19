using Ecommerce.ProductCatalog.Model;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProductCatalog
{
    internal class ServiceFabricProductRepository : IProductRepository
    {
        private readonly IReliableStateManager reliableStateManager;

        public ServiceFabricProductRepository(IReliableStateManager reliableStateManager)
        {
            this.reliableStateManager = reliableStateManager;
        }

        public async Task AddProduct(Product product)
        {
            IReliableDictionary<Guid, Product> productDictionary =
                await this.reliableStateManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>("Products");

            // All service fabric operations need transaction.
            using(ITransaction tx = reliableStateManager.CreateTransaction())
            {
                await productDictionary.AddOrUpdateAsync(tx, product.Id, product, (id, value) => product);
                await tx.CommitAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            IReliableDictionary < Guid, Product > products =
                await reliableStateManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>("Products");
            var result = new List<Product>();

            using (ITransaction tx = reliableStateManager.CreateTransaction())
            {
                var allProducts =
                await products.CreateEnumerableAsync(tx, EnumerationMode.Unordered);
                using (var enumerator = allProducts.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        KeyValuePair < Guid, Product > current = enumerator.Current;
                        result.Add(current.Value);
                    }
                }
            }
            return result;
        }
    }
}
