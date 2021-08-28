namespace PDFTextMining.Pipelining
{
    public interface IFilter
    {
        dynamic Execute(dynamic inputData);
    }
}