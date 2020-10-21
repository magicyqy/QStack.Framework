using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace QStack.Framework.Core.Config
{
    public class InterpolationConfigurationProvider : IConfigurationProvider
    {
        private readonly Regex _variablePattern;
        private readonly IConfiguration _innerConfig;
        protected IDictionary<string, string> Data { get; set; }

        public InterpolationConfigurationProvider(IConfiguration configuration, Regex pattern)
        {
            _innerConfig = configuration;
            _variablePattern = pattern;
        }
        public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
        {
            var prefix = parentPath == null ? string.Empty : parentPath + ConfigurationPath.KeyDelimiter;

            return Data
                .Where(kv => kv.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .Select(kv => Segment(kv.Key, prefix.Length))
                .Concat(earlierKeys)
                .OrderBy(k => k, ConfigurationKeyComparer.Instance);
        }

        private static string Segment(string key, int prefixLength)
        {
            var indexOf = key.IndexOf(ConfigurationPath.KeyDelimiter, prefixLength, StringComparison.OrdinalIgnoreCase);
            return indexOf < 0 ? key.Substring(prefixLength) : key.Substring(prefixLength, indexOf - prefixLength);
        }

        public IChangeToken GetReloadToken()
        {
            return _innerConfig.GetReloadToken();
        }

        public void Load()
        {
            Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var config in _innerConfig.AsEnumerable())
            {
                var newValue = config.Value;
                if (!string.IsNullOrWhiteSpace(newValue))
                {
                    newValue = _variablePattern.Replace(newValue, (m) => {
                        var replacementValue = m.Value;
                        var variableName = m.Groups[1].Value;
                        if (!string.IsNullOrWhiteSpace(variableName))
                        {
                            replacementValue = _innerConfig.GetValue<string>(config.Key + ":" + variableName);
                        }
                        return replacementValue;
                    });
                }
                Data.Add(config.Key, newValue);
            }
        }

        public void Set(string key, string value)
        {
            Data[key] = value;
        }

        public bool TryGet(string key, out string value)
        {
            return Data.TryGetValue(key, out value);
        }
    }
}
