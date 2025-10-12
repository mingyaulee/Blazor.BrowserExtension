using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JsBind.Net;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using WebExtensions.Net;

namespace Blazor.BrowserExtension
{
    /// <summary>
    /// Inherit from the this class and add the [BackgroundWorker] to allow the source generator to fully support generated background worker.
    /// </summary>
    public abstract class BackgroundWorkerBase : IComponent
    {
        protected virtual Dictionary<string, object> JsInstance => throw new InvalidOperationException("The JsInstance should be overridden by the generated code.");

        bool isInitialized;

        [Inject]
        private IJsRuntimeAdapter JsRuntimeAdapter { get; set; }

        [Inject]
        public IWebExtensionsApi WebExtensions { get; set; }

        [Inject]
        public ILogger<BackgroundWorkerBase> Logger { get; set; }

        /// <summary>
        /// Implement the main method and decorate with [BackgroundWorkerMain] attribute.
        /// </summary>
        public abstract void Main();

        /// <summary>
        /// Invoked after the background worker is initialized from JS.
        /// </summary>
        public virtual void OnInitialized()
        {
        }

        void IComponent.Attach(RenderHandle renderHandle)
        {
            if (isInitialized)
            {
                throw new InvalidOperationException("Background worker can only be initialized once.");
            }

            isInitialized = true;
            Initialize();
        }

        Task IComponent.SetParametersAsync(ParameterView parameters)
            => Task.CompletedTask;

        private void Initialize()
        {
            using var initializer = new JsInitializer(JsRuntimeAdapter);
            initializer.Initialize(JsInstance);
            OnInitialized();
        }

        private sealed class JsInitializer : ObjectBindingBase
        {
            public JsInitializer(IJsRuntimeAdapter jsRuntime)
            {
                Initialize(jsRuntime);
                SetAccessPath("globalThis");
            }

            public void Initialize(Dictionary<string, object> jsInstance)
                => InvokeVoid("setBackgroundWorkerInstance", jsInstance);
        }
    }
}
