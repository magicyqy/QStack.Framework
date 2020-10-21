using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace QStack.Framework.Core.Config
{
    public class SwarmSecretsConfigurationProvider : ConfigurationProvider
    {
        private readonly IEnumerable<SwarmSecretsPath> _secretsPaths;

        public SwarmSecretsConfigurationProvider(IEnumerable<SwarmSecretsPath> secretsPaths)
        {
            _secretsPaths = secretsPaths;
        }

        public override void Load()
        {
            var data = new Dictionary<string, string>
                (StringComparer.OrdinalIgnoreCase);

            //Loop through all the files 
            foreach (var secretsPath in _secretsPaths)
            {
                if (!Directory.Exists(secretsPath.Path) && !secretsPath.Optional)
                {
                    throw new FileNotFoundException(secretsPath.Path);
                }
                foreach (var filePath in Directory.GetFiles(secretsPath.Path))
                {
                    var configurationKey = Path.GetFileName(filePath);
                    // if key delimiter is not : the replace 
                    //if (secretsPath.KeyDelimiter != ":")
                    //{
                        // TODO: check how to escape the delimiter
                        configurationKey = configurationKey
                            .Replace(secretsPath.KeyDelimiter, ":");
                    //}
                    // read file content
                    var configurationValue = File.ReadAllText(filePath);
                    data.Add(configurationKey, configurationValue);
                }
            }
            Data = data;
        }
    }
}
