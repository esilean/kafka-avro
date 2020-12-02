using KafkaStudy.Common.Avro;
using KafkaStudy.Common.Messages;
using KafkaStudy.Producer.Builder.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaStudy.Producer.Workers
{
    public class OrderCreatedWorker : BackgroundService
    {
        private readonly ILogger<OrderCreatedWorker> _logger;
        private readonly IMessageProducer<OrderCreatedAvroMessage> _orderCreatedProducer;

        public OrderCreatedWorker(ILogger<OrderCreatedWorker> logger,
                      IMessageProducer<OrderCreatedAvroMessage> orderCreatedProducer)
        {
            _logger = logger;
            _orderCreatedProducer = orderCreatedProducer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //order-created
                var fakeOrder = CreateFakeOrder();
                await _orderCreatedProducer.ProduceAsync(fakeOrder, fakeOrder.Id, stoppingToken);
                _logger.LogInformation("Message: {Id}, {CustomerName}, {CartValue}", fakeOrder.Id, fakeOrder.CustomerName, fakeOrder.CartValue);



                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        private OrderCreatedMessage CreateFakeOrder()
        {
            var message = OrderCreatedMessage
                            .Factory.Create(
                                            Guid.NewGuid().ToString(),
                                            Faker.Name.FullName(),
                                            Faker.RandomNumber.Next(100),
                                            Faker.RandomNumber.Next(10),
                                            (double)Faker.Finance.Coupon());
            return message;
        }


    }
}
