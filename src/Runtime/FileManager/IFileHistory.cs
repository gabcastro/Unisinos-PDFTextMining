using PDFTextMining.Pipelining;

namespace PDFTextMining.Runtime.FileManager
{
    internal interface IFileHistory : IFilter
    {
        FileResponse Generate(FileRequest request);
    }

    class FileRequest
    {
        public string DirPath { get; set; }
        public string FileName { get; set; }
    }

    class FileResponse
    {
        public string Val { get; set; }
    }
}