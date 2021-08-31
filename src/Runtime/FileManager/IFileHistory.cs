using System.Collections.Generic;
using PDFTextMining.Pipelining;

namespace PDFTextMining.Runtime.FileManager
{
    internal interface IFileHistory : IFilter
    {
        FileResponse Generate(FileRequest request);
    }

    class FileRequest
    {
        public string PdfName { get; set; }
        public string QueryString { get; set; }
        public string OccurrencesWords { get; set; }
    }

    class FileResponse
    {
        public string Val { get; set; }
    }
}