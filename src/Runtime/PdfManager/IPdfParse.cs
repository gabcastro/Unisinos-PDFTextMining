using System.Collections.Generic;
using PDFTextMining.Pipelining;

namespace PDFTextMining.Runtime.PdfManager
{
    internal interface IPdfParse : IFilter
    {
        PdfParseResponse Generate(PdfParseRequest request);
    }

    class PdfParseRequest
    {
        public string PdfPath { get; set; }
        public string QueryString { get; set; }
    }

    class PdfParseResponse
    {
        public string PdfName { get; set; }
        public string QueryString { get; set; }
        public string WordsCount { get; set; }
    }

}