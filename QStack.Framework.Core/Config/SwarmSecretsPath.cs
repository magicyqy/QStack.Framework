using System;

namespace QStack.Framework.Core.Config
{
    public class SwarmSecretsPath
    {
        public SwarmSecretsPath()
        {
        }
        public SwarmSecretsPath(
            string path,
            string keyDelimiter = "_",//默认secret文件名用“_”分隔
            bool optional = false,
            Func<string, string> keySelector = null)
        {
            Path = path;
            KeyDelimiter = keyDelimiter;
            Optional = optional;
            KeySelector = keySelector;
        }
        public string Path { get; set; }
        public string KeyDelimiter { get; set; }
        public bool Optional { get; set; }
        public Func<string, string> KeySelector { get; set; }

    }
}
