using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PDFTextMining.Runtime.PdfManager
{
    class PdfReader : IPdfReader
    {
        private readonly ILogger<PdfReader> _logger;
        private readonly IConfigurationRoot _config;
        
        public PdfReader(IConfigurationRoot config, ILoggerFactory loggerFactory)
        {
            _config = config;
            _logger = loggerFactory.CreateLogger<PdfReader>();
        }

        public dynamic Execute(dynamic inputData)
        {
            return Generate(inputData);
        }

        public PdfReaderResponse Generate(PdfReaderRequest request)
        {
            return new PdfReaderResponse { PdfName = "request.PdfName" };
        }
    }
}