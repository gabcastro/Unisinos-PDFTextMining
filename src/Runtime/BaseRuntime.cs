using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PDFTextMining.Runtime
{
    public class BaseRuntime
    {
        public ILogger<BaseRuntime> Logger { get; private set; }
        public IConfigurationRoot Config { get; private set; }

        public BaseRuntime()
        {
        }

        public BaseRuntime(IConfigurationRoot config, ILoggerFactory loggerFactory)
        {
            Config = config;
            Logger = loggerFactory.CreateLogger<BaseRuntime>();
        }
    }
}