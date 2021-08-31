using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PDFTextMining.Runtime.FileManager
{
    internal class FileHistory : IFileHistory
    {
        private readonly ILogger<FileHistory> _logger;
        private readonly IConfigurationRoot _config;
        public int IdSearch { get; private set; }

        public FileHistory(IConfigurationRoot config, ILoggerFactory loggerFactory)
        {
            _config = config;
            _logger = loggerFactory.CreateLogger<FileHistory>();
            IdSearch = 0;
        }

        public dynamic Execute(dynamic inputData)
        {
            return Generate(inputData);
        }

        public FileResponse Generate(FileRequest request)
        {
            string logFileName = _config["LogFileName"];

            if (!File.Exists(logFileName))
                File.Create(logFileName);

            try
            {
                ReadFile(logFileName);
                SaveSearchResult(logFileName, request.PdfName, request.QueryString, request.OccurrencesWords);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
                throw e;
            }


            _logger.LogInformation("Novo registro terá ID: {0}", IdSearch);

            return new FileResponse { Val = 0.ToString() };
        }

        private void ReadFile(string fullPathFile)
        {
            using StreamReader file = new StreamReader(fullPathFile);
            string ln;
            while ((ln = file.ReadLine()) != null)
            {
                if (ln.Contains("Número da consulta:"))
                    IdSearch = Int32.Parse(ln.Trim().Split(':')[1]);
            }
            file.Close();

            IdSearch += 1;
        }

        private void SaveSearchResult(string fullPathFile, string pdfName, string queryString, string occurrencesWords)
        {
            StreamWriter streamWriter = new StreamWriter(path: fullPathFile, append: true, encoding: Encoding.UTF8);
            using StreamWriter file = streamWriter;
            file.WriteLine("****************************************");
            file.WriteLine("Número da consulta: {0}", IdSearch);
            file.WriteLine("Nome do documento: {0}", pdfName);
            file.WriteLine("String de busca: {0}", queryString);
            file.WriteLine("Ocorrências: {0}", occurrencesWords);
            file.WriteLine("****************************************");
        }
    }
}