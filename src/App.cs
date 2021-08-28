using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PDFTextMining.Pipelining;
using PDFTextMining.Runtime.FileManager;
using System.Threading.Tasks;

namespace PDFTextMining
{
    public class App
    {
        private readonly ILogger<App> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfigurationRoot _config;

        public App(IConfigurationRoot config, ILoggerFactory loggerFactory)
        {
            _config = config;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<App>();
        }

        public async Task Execute()
        {
            _logger.LogInformation("Pipeline setup...");

            var p = new Pipeline<FileRequest, FileResponse>()
                .Add(new FileHistory(_config, _loggerFactory));

            p.PipelineExecuting += PipelineExecutingLog;

            using (p) 
            {
                // first request
                var request = new FileRequest
                {
                    DirPath = _config["DirPath"],
                    FileName = _config["LogFileName"]
                };

                p.Execute(request);
            }    
        }

        protected void PipelineExecutingLog(IFilter filter)
        {
            var msg = $"Step: {filter.GetType().Name}";

            _logger.LogInformation(msg);
        }
    }
}