using Azure;
using Azure.Data.Tables;
using ABCRetail.Models;

namespace ABCRetail.Services
{
    public class CustomerTableService
    {
        private readonly TableClient _tableClient;

        public CustomerTableService(TableServiceClient tableServiceClient) 
        {
            _tableClient = tableServiceClient.GetTableClient("Customers");
            _tableClient.CreateIfNotExists();
        }

        public async Task AddCustomerAsync(CustomerEntity customer)
        {
            await _tableClient.AddEntityAsync(customer);
        }

        public async Task<List<CustomerEntity>> GetCustomersAsync()
        {
            var results = new List<CustomerEntity>();
            await foreach (var entity in _tableClient.QueryAsync<CustomerEntity>())
            {
                results.Add(entity);
            }
            return results;
        }

        public async Task<CustomerEntity?> GetCustomerAsync(string partitionKey, string rowKey)
        {
            try
            {
                var response = await _tableClient.GetEntityAsync<CustomerEntity>(partitionKey, rowKey);
                return response.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }

        public async Task UpdateCustomerAsync(CustomerEntity customer)
        {
            await _tableClient.UpdateEntityAsync(customer, customer.ETag, TableUpdateMode.Replace);
        }

        public async Task DeleteCustomerAsync(string partitionKey, string rowKey)
        {
            await _tableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

        public async Task<bool> CustomerExistsAsync(string partitionKey, string rowKey)
        {
            try
            {
                var response = await _tableClient.GetEntityAsync<CustomerEntity>(partitionKey, rowKey);
                return response != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<CustomerEntity>> GetCustomerByPartitionAsync(string partitionKey)
        {
            var results = new List<CustomerEntity>();
            await foreach (var entity in _tableClient.QueryAsync<CustomerEntity>(c => c.PartitionKey == partitionKey))
            {
                results.Add(entity);
            }
            return results;
        }
    }
}
