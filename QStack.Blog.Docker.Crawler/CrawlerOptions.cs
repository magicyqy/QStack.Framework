using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Blog.Docker.Crawler
{
    public class CrawlerOptions
    {

        private readonly IConfiguration _configuration;

        public CrawlerOptions(IConfiguration configuration)
        {
            _configuration = configuration;
           
        }

     

        public string Docker => _configuration["Docker"];

        public string[] DockerVolumes =>
            string.IsNullOrWhiteSpace(_configuration["DockerVolumes"])
                ? new string[0]
                : _configuration["DockerVolumes"].Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        public string RabbitMQ_HostName=> _configuration["RabbitMQ:HostName"];
        public string RabbitMQ_UserName => _configuration["RabbitMQ:UserName"];
        public string RabbitMQ_Password => _configuration["RabbitMQ:Password"];
        public string RabbitMQ_Exchange => _configuration["RabbitMQ:Exchange"];
    }

}
