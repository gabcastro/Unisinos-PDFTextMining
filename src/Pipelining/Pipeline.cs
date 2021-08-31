using System;
using System.Collections.Generic;
using System.Linq;

namespace PDFTextMining.Pipelining
{
    class Pipeline<TInput, TOutput> : IDisposable
    {
        public event PipelineEventHandler PipelineExecuting;
        public delegate void PipelineEventHandler (IFilter filter);

        protected Queue<IFilter> Filters { get; private set; }

        public Pipeline()
        {
            Filters = new Queue<IFilter>();    
        }

        /// <summary>
        /// Enqueue a new function that will be call from the queue
        /// </summary>
        public Pipeline<TInput, TOutput> Add(IFilter filter)
        {
            Filters.Enqueue(filter);

            return this;
        }

        /// <summary>
        /// Enqueue a new function, but with a respect type as parameter to return a new type that will be use in the next step 
        /// </summary>
        public Pipeline<TInput, TOutput> Add<T1, T2>(Func<T1, T2> filter)
        {
            var wrappedFilter = new LambdaFilter<T1, T2>(filter);
            Filters.Enqueue(wrappedFilter);

            return this;
        }

        public TOutput Execute(TInput inputData)
        {
            dynamic result = null, args = inputData;

            while (Filters.Any())
            {
                var step = Filters.Dequeue();

                result = ExecuteFilter(step, args);
                args = result;
            }

            return result;
        }

        protected virtual dynamic ExecuteFilter(IFilter step, dynamic args)
        {
            dynamic result = step.Execute(args);

            return result;
        }

        #region IDisposable Members

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    Filters = null;
                    PipelineExecuting = null;
                }
            }

            this._disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}