using PDFTextMining.Pipelining;

namespace PDFTextMining.Runtime.PdfManager
{
    internal interface IPdfReader : IFilter
    {
        PdfReaderResponse Generate(PdfReaderRequest request);
    }

    class PdfReaderRequest
    {
        public string PdfName { get; set; }
    }

    class PdfReaderResponse
    {
        public string PdfName { get; set; }
    }

}