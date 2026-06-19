using Confluent.Kafka;

namespace ChineseSaleConsumer
{
    public class KafkaConsumerWorker : BackgroundService
    {
        private readonly ILogger<KafkaConsumerWorker> _logger;
        private readonly IConfiguration _configuration;

        public KafkaConsumerWorker(ILogger<KafkaConsumerWorker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
                GroupId = "chinese-sale-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };

            var topic = _configuration["Kafka:Topic"] ?? "chinese-sale-events";

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe(topic);

            _logger.LogInformation("Kafka Consumer started. Listening on topic: {Topic}", topic);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var result = consumer.Consume(stoppingToken);

                        _logger.LogInformation(
                            "Message received: Key={Key}, Value={Value}, Partition={Partition}, Offset={Offset}",
                            result.Message.Key,
                            result.Message.Value,
                            result.Partition.Value,
                            result.Offset.Value);
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Error consuming Kafka message");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Kafka Consumer shutting down.");
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}
