using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PDFTextMining.Pipelining;
using PDFTextMining.Runtime.FileManager;
using PDFTextMining.Runtime.PdfManager;
using System;
using System.IO;
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
            _logger.LogInformation("Digite o caminho do arquivo .pdf");
            // string pdfPath = Console.ReadLine();
            string pdfPath = @"D:\Downloads\coisas pessoais\teste.pdf";

            if (!File.Exists(path: pdfPath))
            {
                _logger.LogError("Caminho inválido, nenhum pdf com esse nome encontrado!");
                throw new Exception();
            }

            _logger.LogInformation("Digite a string de busca");
            // string queryString = Console.ReadLine();
            string queryString = "Aplicação";

            Generate(pdfPath, queryString);
        }

        private void Generate(string pdfPath, string queryString)
        {
            _logger.LogInformation("Pipeline setup...");

            var p = new Pipeline<PdfParseRequest, FileResponse>()
                .Add(new PdfParse(_config, _loggerFactory))
                .Add<PdfParseResponse, FileRequest>(Convert)
                .Add(new FileHistory(_config, _loggerFactory))
            ;

            p.PipelineExecuting += PipelineExecutingLog;

            using (p) 
            {
                // first request
                var request = new PdfParseRequest
                {
                    PdfPath = pdfPath,
                    QueryString = queryString
                };

                p.Execute(request);
            }    
        }

        private FileRequest Convert(PdfParseResponse input)
        {
            return new FileRequest
            {
                PdfName = input.PdfName,
                QueryString = input.QueryString,
                OccurrencesWords = input.WordsCount
            };
        }

        protected void PipelineExecutingLog(IFilter filter)
        {
            var msg = $"Step: {filter.GetType().Name}";

            _logger.LogInformation(msg);
        }
    }
}