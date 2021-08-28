using System;

namespace PDFTextMining.Pipelining
{
    class LambdaFilter<TInput, TOutput> : IFilter
    {
        private Func<TInput, TOutput> call;

        public LambdaFilter(Func<TInput, TOutput> func)
        {
            this.call = func;
        }

        public dynamic Execute(dynamic inputData)
        {
            return call(inputData);
        }
    }
}