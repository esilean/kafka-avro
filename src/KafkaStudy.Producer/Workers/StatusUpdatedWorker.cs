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
    public class StatusUpdatedWorker : BackgroundService
    {
        private readonly ILogger<StatusUpdatedWorker> _logger;
        private readonly IMessageProducer<StatusUpdatedAvroMessage> _statusUpdatedProducer;

        public StatusUpdatedWorker(ILogger<StatusUpdatedWorker> logger,
                      IMessageProducer<StatusUpdatedAvroMessage> statusUpdatedProducer)
        {
            _logger = logger;
            _statusUpdatedProducer = statusUpdatedProducer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int auxValue = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                //status-updated
                var fakeStatus = CreateFakeStatus(auxValue);
                await _statusUpdatedProducer.ProduceAsync(fakeStatus, fakeStatus.Id, stoppingToken);
                _logger.LogInformation("Message: {Id}, {Status}", fakeStatus.Id, fakeStatus.Status);

                _logger.LogInformation("StatusUpdatedWorker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
                auxValue++;
            }
        }

        private StatusUpdatedMessage CreateFakeStatus(int auxValue)
        {
            var status = (auxValue % 2 == 0) ? "Processado" : "Cancelado";
            var message = StatusUpdatedMessage.Factory.Create(Guid.NewGuid().ToString(), status);
            return message;
        }


    }
}
