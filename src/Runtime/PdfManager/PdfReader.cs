namespace PDFTextMining.Runtime.PdfManager
{
    class PdfReader : IPdfReader
    {
        public PdfReader()
        {
            
        }

        public dynamic Execute(dynamic inputData)
        {
            return Generate(inputData);
        }

        public PdfReaderResponse Generate(PdfReaderRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}