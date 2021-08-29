using PDFTextMining.Pipelining;

namespace PDFTextMining.Runtime.PdfManager
{
    internal interface IPdfReader : IFilter
    {
        PdfReaderResponse Generate(PdfReaderRequest request);
    }

    class PdfReaderRequest
    {
        public string PdfPath { get; set; }
        public string QueryString { get; set; }
    }

    class PdfReaderResponse
    {
        public string PdfName { get; set; }
    }

}