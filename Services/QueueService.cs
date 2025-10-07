using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Configuration.EnvironmentVariables;

namespace ABCRetail.Services
{
    public class QueueService
    {
        private readonly QueueServiceClient _queueServiceClient;

        public QueueService(QueueServiceClient queueServiceClient)
        {
            _queueServiceClient = queueServiceClient;
        }

        public async Task EnqueueMessageAsync(string queueName, string message)
        {
            try
            {
                var queueClient = _queueServiceClient.GetQueueClient(queueName.ToLower());
                await queueClient.CreateIfNotExistsAsync();
                await queueClient.SendMessageAsync(message);
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"Error adding message: {ex.Message}");
                throw;
            }
            
        }

        public async Task<string?> DequeueMessageAsync(string queueName)
        {
            try
            {
                var queueClient = _queueServiceClient.GetQueueClient(queueName.ToLower());
                await queueClient.CreateIfNotExistsAsync();

                QueueMessage[] messages = await queueClient.ReceiveMessagesAsync(1); 

				if (messages.Length > 0)
                {
                    var message = messages[0];
                    await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                    return message.MessageText;
                }
            return null;
            } 
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"Error dequeuing message: {ex.Message}");
                throw;
            }
            
        }

        public async Task<string?> PeekMessageAsync(string queueName)
        {
            try
            {
                var queueClient = _queueServiceClient.GetQueueClient(queueName.ToLower());
                await queueClient.CreateIfNotExistsAsync();

                PeekedMessage[] peekedMessages = await queueClient.PeekMessagesAsync(1);
                return peekedMessages.Length > 0 ? peekedMessages[0].MessageText : null;
            }
            catch(RequestFailedException ex)
            {
                Console.WriteLine($"Error peeking message: {ex.Message}");
                throw;
            }
        }

        public async Task<List<string>> GetAllMessagesAsync(string queueName)
        {
            var result = new List<string>();
            var queueClient = _queueServiceClient.GetQueueClient(queueName.ToLower());
            await queueClient.CreateIfNotExistsAsync();

            PeekedMessage[] peekedMessages = await queueClient.PeekMessagesAsync(32); 
            foreach ( var message in peekedMessages )
            {
                result.Add(message.MessageText);
            }

            return result;
        }

        public async Task ClearQueueAsync(string queueName)
        {
            var queueClient = _queueServiceClient.GetQueueClient(queueName.ToLower());
            await queueClient.CreateIfNotExistsAsync();
            await queueClient.ClearMessagesAsync();
        }
    }
}
