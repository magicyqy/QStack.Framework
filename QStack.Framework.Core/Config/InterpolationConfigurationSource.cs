using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace QStack.Framework.Core.Config
{
    public class InterpolationConfigurationSource : IConfigurationSource
    {
        private readonly Regex _pattern = new Regex("(?:{{(.*?)}})");
        private readonly IConfigurationRoot _configuration;
        public InterpolationConfigurationSource(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new InterpolationConfigurationProvider(_configuration, _pattern);
        }
    }
}