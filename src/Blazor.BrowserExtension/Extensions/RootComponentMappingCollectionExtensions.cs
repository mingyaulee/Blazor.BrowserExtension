using System;
using Blazor.BrowserExtension;

namespace Microsoft.AspNetCore.Components.WebAssembly.Hosting
{
    public static class RootComponentMappingCollectionExtensions
    {
        public static void AddBackgroundWorker<T>(this RootComponentMappingCollection rootComponents)
            where T : BackgroundWorkerBase
        {
            rootComponents.AddBackgroundWorker(typeof(T));
        }

        public static void AddBackgroundWorker(this RootComponentMappingCollection rootComponents, Type backgroundWorkerType)
        {
            rootComponents.Add(backgroundWorkerType, "#background");
        }
    }
}
