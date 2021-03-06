﻿using AutoMapper;
using KafkaStudy.Common;
using KafkaStudy.Common.Avro;
using KafkaStudy.Common.Messages;
using KafkaStudy.Consumer.Builder;
using KafkaStudy.Consumer.Builder.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace KafkaStudy.Consumer.Infra
{
    public static class ServiceExtensions
    {
        public delegate IKafkaTopicMessageConsumer ServiceResolver(string key);

        public static IServiceCollection AddKafkaConsumer(this IServiceCollection services,
            params Type[] handlerAssemblyMarkerTypes)
        {
            services.AddMediatR(handlerAssemblyMarkerTypes);
            services.AddAutoMapper(handlerAssemblyMarkerTypes);

            services.AddTransient<IKafkaMessageConsumerManager>(serviceProvider =>
                                    new KafkaMessageConsumerManager(serviceProvider, services));

            //status-updated
            services.AddTransient<KafkaTopicMessageConsumer<StatusUpdatedAvroMessage, StatusUpdatedMessage>>();
            services.AddTransient<IKafkaConsumerBuilder<StatusUpdatedAvroMessage>, KafkaConsumerBuilder<StatusUpdatedAvroMessage>>();

            //order-created
            services.AddTransient<KafkaTopicMessageConsumer<OrderCreatedAvroMessage, OrderCreatedMessage>>();
            services.AddTransient<IKafkaConsumerBuilder<OrderCreatedAvroMessage>, KafkaConsumerBuilder<OrderCreatedAvroMessage>>();

            services.AddTransient<ServiceResolver>(serviceProvider => key =>
            {
                return key switch
                {
                    Topics.STATUS_UPDATED => serviceProvider.GetService<KafkaTopicMessageConsumer<StatusUpdatedAvroMessage, StatusUpdatedMessage>>(),
                    Topics.ORDER_CREATED => serviceProvider.GetService<KafkaTopicMessageConsumer<OrderCreatedAvroMessage, OrderCreatedMessage>>(),
                    _ => throw new KeyNotFoundException(),
                };
            });

            return services;
        }
    }
}
