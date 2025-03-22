using System;

namespace Blazor.BrowserExtension.Analyzer.Translation
{
    internal class ParentExpressionTypeSwitcher : IDisposable
    {
        private readonly TranslateContext context;
        private readonly ParentExpressionType originalType;
        private bool disposedValue;

        public ParentExpressionTypeSwitcher(TranslateContext context, ParentExpressionType type)
        {
            this.context = context;
            originalType = context.ParentExpressionType;
            context.ParentExpressionType = type;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    context.ParentExpressionType = originalType;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
