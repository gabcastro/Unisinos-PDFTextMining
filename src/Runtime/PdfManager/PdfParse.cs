using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace PDFTextMining.Runtime.PdfManager
{
    internal class PdfParse : IPdfParse
    {
        private readonly ILogger<PdfParse> _logger;
        private readonly IConfigurationRoot _config;
        private Dictionary<string, int> KeyWords;
        
        public PdfParse(IConfigurationRoot config, ILoggerFactory loggerFactory)
        {
            _config = config;
            _logger = loggerFactory.CreateLogger<PdfParse>();
            KeyWords = new Dictionary<string, int>();
        }

        public dynamic Execute(dynamic inputData)
        {
            return Generate(inputData);
        }

        public PdfParseResponse Generate(PdfParseRequest request)
        {
            try
            {
                KeyWords = request.KeyWords;

                CountOccurrencesWords(request.PdfPath);
                CheckOperation(request.Operation);

                var lisNamePdf = request.PdfPath.Split(@"\");

                return new PdfParseResponse {
                    PdfName = lisNamePdf[^1],
                    QueryString = request.QueryString,
                    WordsCount = ToString(KeyWords)
                };   
            }
            catch (Exception e)
            {
                _logger.LogError("Error message: {0}", e.Message);
                throw;
            }
        }

        /// <summary>
        /// Responsability:
        ///     Read pdf file
        ///     Remove accents from all words
        ///     Compare with query string and count frequency of each word
        /// </summary>
        private void CountOccurrencesWords(string pathPdf)
        {
            var tempDict = new Dictionary<string, int>();

            var pdfDocument = new PdfDocument(new PdfReader(filename: pathPdf));
            var strategy = new LocationTextExtractionStrategy();
            
            for (int i = 1; i <= pdfDocument.GetNumberOfPages(); ++i)
            {
                var page = pdfDocument.GetPage(i);
                string text = PdfTextExtractor.GetTextFromPage(page, strategy);
                string[] lines = text.Split('\n');
                
                foreach(string line in lines)
                {
                    foreach (var k in KeyWords)
                    {
                        string lowerCaseTextString = RemoveDiacritics(line.ToLower());
                        string lowerCaseWord = RemoveDiacritics(k.Key.ToLower());

                        int index = 0;
                        int diffLineWord = lowerCaseTextString.Length - lowerCaseWord.Length;

                        while (diffLineWord >= index && index != -1)
                        {
                            index = lowerCaseTextString.IndexOf(lowerCaseWord, index);
                            if (index != -1) 
                            {
                                if (tempDict.ContainsKey(k.Key))
                                    tempDict[k.Key] += 1;
                                else 
                                    tempDict.Add(k.Key, 1);
                                index = lowerCaseWord.Length + index;
                            }
                        }
                    }
                }
            }

            foreach (var i in tempDict)
                KeyWords[i.Key] = i.Value;
        }

        /// <summary>
        /// If is an operation of AND, than will be set to zero all elements
        /// </summary>
        private void CheckOperation(int op)
        {
            var tempDict = new Dictionary<string, int>();

            if (op == 1) 
            {
                bool setZeroAll = false;

                foreach (var i in KeyWords)
                {
                    if (i.Value == 0)
                    {
                        setZeroAll = true;
                        break;
                    }
                }

                if (setZeroAll) 
                {
                    foreach (var i in KeyWords) 
                        tempDict.Add(i.Key, 0);
            
                    KeyWords = tempDict;
                }
            }
        }

        private string ToString(Dictionary<string, int> occurrencesWords)
        {
            string strOccurrences = "";

            foreach(var i in occurrencesWords)
                strOccurrences = strOccurrences + i.Key + "(" + i.Value + "), ";

            return strOccurrences[0..^2];
        }

        /// <summary>
        /// Remove accents from a string
        /// <summary>
        private string RemoveDiacritics(string text) 
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}