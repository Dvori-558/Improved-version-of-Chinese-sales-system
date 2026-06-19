using Confluent.Kafka;
using System.Text.Json;
 
namespace ChineseSaleApi.Services
{
    public interface IKafkaProducerService
    {
        Task SendMessageAsync<T>(string key, T message);
    }
 
    public class KafkaProducerService : IKafkaProducerService, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _topic;
        private readonly ILogger<KafkaProducerService> _logger;
 
        public KafkaProducerService(IConfiguration configuration, ILogger<KafkaProducerService> logger)
        {
            _logger = logger;
            _topic = configuration["Kafka:Topic"] ?? "chinese-sale-events";
 
            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
                MessageTimeoutMs = 5000,
                RequestTimeoutMs = 3000
            };
 
            _producer = new ProducerBuilder<string, string>(config).Build();
        }
 
        public async Task SendMessageAsync<T>(string key, T message)
        {
            try
            {
                var json = JsonSerializer.Serialize(message);
                _producer.Produce(_topic, new Message<string, string>
                {
                    Key = key,
                    Value = json
                }, deliveryReport =>
                {
                    if (deliveryReport.Error.IsError)
                    {
                        _logger.LogWarning("Kafka delivery failed: {Error}", deliveryReport.Error.Reason);
                    }
                    else
                    {
                        _logger.LogInformation("Kafka message sent: Topic={Topic}, Key={Key}, Offset={Offset}",
                            deliveryReport.Topic, key, deliveryReport.Offset);
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send Kafka message: Key={Key}", key);
                // Don't throw - Kafka failure shouldn't break the main flow
            }
        }
 
        public void Dispose()
        {
            _producer?.Dispose();
        }
    }
}
 
 