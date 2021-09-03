using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PDFTextMining.Pipelining;
using PDFTextMining.Runtime.FileManager;
using PDFTextMining.Runtime.PdfManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
            _logger.LogInformation("Type the .pdf path");
            string pdfPath = Console.ReadLine();
            // string pdfPath = @"D:\Downloads\coisas pessoais\teste.pdf";

            if (!File.Exists(path: pdfPath))
                throw new Exception("Invalid path, pdf not found!");

            _logger.LogInformation("Type a query string");
            string queryString = Console.ReadLine();

            if (string.IsNullOrEmpty(queryString))
                throw new Exception("Query string empty");

            // string queryString = "Aplicação AND implementação AND poderá AND algo";

            var keyWords = new Dictionary<string, int>();
            int op;

            (keyWords, op) = CheckQueryString(queryString);
            Generate(pdfPath, queryString, keyWords, op);
        }

        private void Generate(string pdfPath, string queryString, Dictionary<string, int> keyWords, int operation)
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
                    QueryString = queryString,
                    KeyWords = keyWords,
                    Operation = operation
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

        /// <summary>
        /// Check if query consist of an operation OR or AND
        /// in case that there are two different operations, an exception is throwing     
        /// </summary>
        private Tuple<Dictionary<string, int>, int> CheckQueryString(string queryString)
        {
            int op;
            var keyWords = new Dictionary<string, int>();

            if (Regex.IsMatch(queryString, @"\bAND\b") && Regex.IsMatch(queryString, @"\bOR\b"))
                throw new Exception("Not allowed two operations in a query string");
            else if (Regex.IsMatch(queryString, @"\bAND\b") || Regex.IsMatch(queryString, @"\bOR\b"))
                (keyWords, op) = SplitQuery(queryString);
            else {
                keyWords.Add(queryString, 0);
                op = 0;
            } 
            
            return Tuple.Create(keyWords, op);
        }

        /// <summary>
        /// This method will return a tuple with:
        ///     a dictionary that represent each word to find 
        ///     an integer that represent what operation is
        /// </summary>
        private Tuple<Dictionary<string, int>, int> SplitQuery(string queryString)
        {
            var keyValues = new Dictionary<string, int>();
            int op;

            if (Regex.IsMatch(queryString, @"\bAND\b"))
            {
                foreach (var i in queryString.Split("AND"))
                {
                    if (!i.Trim().Any(Char.IsUpper))
                        keyValues.Add(i.Trim(), 0);
                    else
                        throw new Exception("String must to be entire lower case");
                }
                op = 1;
            }
            else
            {
                foreach(var i in queryString.Split("OR"))
                {
                    if (!i.Trim().Any(Char.IsUpper))
                        keyValues.Add(i.Trim(), 0);
                    else 
                        throw new Exception("String must to be entire lower case");
                }
                op = 2;
            } 

            return Tuple.Create(keyValues, op);
        }

        protected void PipelineExecutingLog(IFilter filter)
        {
            var msg = $"Step: {filter.GetType().Name}";

            _logger.LogInformation(msg);
        }
    }
}