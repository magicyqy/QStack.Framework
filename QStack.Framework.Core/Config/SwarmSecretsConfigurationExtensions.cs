using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace QStack.Framework.Core.Config
{
    public static class SwarmSecretsConfigurationExtensions
    {
        public static IConfigurationBuilder AddSwarmSecrets(this IConfigurationBuilder builder)
        {
            builder.Add(new SwarmSecretsConfigurationSource());
            return builder;
        }

        public static IConfigurationBuilder AddSwarmSecrets(this IConfigurationBuilder builder, 
            IEnumerable<SwarmSecretsPath> swarmSecretsPaths)
        {
            builder.Add(new SwarmSecretsConfigurationSource(swarmSecretsPaths));
            return builder;
        }

        public static IConfigurationBuilder AddSwarmSecrets(this IConfigurationBuilder builder, Action<SwarmSecretsConfigurationSource> configureSource)
        {
            return builder.Add(configureSource);
        }
    }
}