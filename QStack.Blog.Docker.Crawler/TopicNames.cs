using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Blog.Docker.Crawler
{
    public static class TopicNames
    {
        public const string AgentCenter = "DOTNET_SPIDER_AGENT_CENTER";
        public const string Agent = "DOTNET_SPIDER_AGENT_{0}";
        public const string Statistics = "DOTNET_SPIDER_STATISTICS_CENTER";
        public const string Spider = "DOTNET_SPIDER_{0}";
        public const string HttpClientAgent = "DOTNET_SPIDER_AGENT_HTTPCLIENT";
        public const string HttpClientWithADSLAgent = "DOTNET_SPIDER_AGENT_HTTPCLIENT_ADSL";
        public const string PuppeteerAgent = "DOTNET_SPIDER_AGENT_PUPPETEER";
        public const string PuppeteerWithADSLAgent = "DOTNET_SPIDER_AGENT_PUPPETEER_ADSL";
    }

}
