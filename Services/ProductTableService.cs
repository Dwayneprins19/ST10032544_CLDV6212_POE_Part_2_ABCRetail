using Azure.Data.Tables;
using ABCRetail.Models;

namespace ABCRetail.Services
{
    public class ProductTableService
    {
        private readonly TableClient _tableClient;

        public ProductTableService(TableServiceClient tableServiceClient)
        {
            _tableClient = tableServiceClient.GetTableClient("Products");
            _tableClient.CreateIfNotExists();
        }

        public async Task AddProductAsync(ProductEntity product)
        {
            await _tableClient.AddEntityAsync(product);
        }

        public async Task<List<ProductEntity>> GetProductsAsync()
        {
            var results = new List<ProductEntity>();
            await foreach(var entity in _tableClient.QueryAsync<ProductEntity>())
            {
                results.Add(entity);
            }
            return results;
        }
        public async Task<ProductEntity?> GetProductAsync(string partitionKey, string rowKey)
        {
            try
            {
                var response = await _tableClient.GetEntityAsync<ProductEntity>(partitionKey, rowKey);
                return response.Value;
            }
            catch
            {
                return null;
            }
        }

        public async Task UpdateProductAsync(ProductEntity product)
        {
            await _tableClient.UpdateEntityAsync(product, product.ETag, TableUpdateMode.Replace);
        }

        public async Task DeleteProductAsync(string partitionKey, string rowKey)
        {
            await _tableClient.DeleteEntityAsync(partitionKey, rowKey);
        }
    }
}
