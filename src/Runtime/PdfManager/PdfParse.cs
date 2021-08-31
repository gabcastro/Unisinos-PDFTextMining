using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace PDFTextMining.Runtime.PdfManager
{
    internal class PdfParse : IPdfParse
    {
        private readonly ILogger<PdfParse> _logger;
        private readonly IConfigurationRoot _config;
        
        public PdfParse(IConfigurationRoot config, ILoggerFactory loggerFactory)
        {
            _config = config;
            _logger = loggerFactory.CreateLogger<PdfParse>();
        }

        public dynamic Execute(dynamic inputData)
        {
            return Generate(inputData);
        }

        public PdfParseResponse Generate(PdfParseRequest request)
        {
            Dictionary<string, int> keyValues = new Dictionary<string, int>();
            int op;

            if (Regex.IsMatch(request.QueryString, @"\bAND\b") || Regex.IsMatch(request.QueryString, @"\bOR\b"))
                (keyValues, op) = SplitQuery(request.QueryString);
            else{
                keyValues.Add(request.QueryString, 0);
                op = 0;
            } 

            keyValues = CountOccurrencesWords(keyValues, request.PdfPath);

            var lisNamePdf = request.PdfPath.Split(@"\");

            return new PdfParseResponse {
                PdfName = lisNamePdf[^1],
                QueryString = request.QueryString,
                WordsCount = ToString(keyValues)
            };
        }

        private Dictionary<string, int> CountOccurrencesWords(Dictionary<string, int> dict, string pathPdf)
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
                    foreach (var k in dict)
                    {
                        // var teste = line.Split(" ").Where(
                        //     p => compareInfo.IndexOf(source: p, value: k.Key, CompareOptions.IgnoreNonSpace) > -1
                        // );
                        // int iii = 0;
                        
                        string lowerCaseTextString = line.ToLower();
                        string lowerCaseWord = k.Key.ToLower();

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
                dict[i.Key] = i.Value;

            return dict;
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
                    keyValues.Add(i, 0);
                op = 1;
            }
            else
            {
                foreach(var i in queryString.Split("OR"))
                    keyValues.Add(i, 0);
                op = 2;
            } 

            return Tuple.Create(keyValues, op);
        }

        private string ToString(Dictionary<string, int> occurrencesWords)
        {
            string strOccurrences = "";

            foreach(var i in occurrencesWords)
                strOccurrences = strOccurrences + i.Key + "(" + i.Value + "), ";

            return strOccurrences[0..^2];
        }
    }
}