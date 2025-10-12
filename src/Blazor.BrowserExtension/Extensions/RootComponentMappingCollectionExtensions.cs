using System;
using Blazor.BrowserExtension;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.AspNetCore.Components.WebAssembly.Hosting
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public static class RootComponentMappingCollectionExtensions
    {
        public static void AddBackgroundWorker<T>(this RootComponentMappingCollection rootComponents)
            where T : BackgroundWorkerBase
            => rootComponents.AddBackgroundWorker(typeof(T));

        public static void AddBackgroundWorker(this RootComponentMappingCollection rootComponents, Type backgroundWorkerType)
            => rootComponents.Add(backgroundWorkerType, "#background");
    }
}
