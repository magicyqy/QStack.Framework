using DotNetCore.CAP;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Savorboard.CAP.InMemoryMessageQueue;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace QStack.Framework.Core.MessageQueue
{
    public static class MessageQueueExtentions
    {
        public static IServiceCollection AddCapInMemoryQueue(this IServiceCollection services,Action<CapOptions> configAction=null)
        {
            services.AddCap(options => { 
               
                options.UseInMemoryMessageQueue();
                options.UseInMemoryStorage();
                configAction?.Invoke(options);
            });
           
            return services;
        }
        public static IServiceCollection AddCapWithRabbitMQ(this IServiceCollection services, IConfiguration configuration, Action<CapOptions> configAction = null)
        {
            var rabbitMQOptions = new RabbitMQOptions();
            configuration.GetSection("RabbitMQOptions").Bind(rabbitMQOptions);
            if (rabbitMQOptions == null)
                throw new ArgumentNullException(nameof(RabbitMQOptions),"can not found the configuration section");
            services.AddCap(options => {

                options.UseRabbitMQ(mqOption=> {
                    mqOption.ExchangeName = rabbitMQOptions.ExchangeName;
                    mqOption.HostName = rabbitMQOptions.HostName;
                    mqOption.Port = rabbitMQOptions.Port;
                    mqOption.UserName = rabbitMQOptions.UserName;
                    mqOption.Password = rabbitMQOptions.Password;
                    mqOption.QueueMessageExpires = rabbitMQOptions.QueueMessageExpires;
                    
                });
                options.UseInMemoryStorage();
                configAction?.Invoke(options);
            });

            return services;
        }
    }
}
