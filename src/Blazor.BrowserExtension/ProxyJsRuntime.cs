using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Blazor.BrowserExtension
{
    internal class ProxyJsRuntime(IJSRuntime instance, BrowserExtensionEnvironment browserExtensionEnvironment) : IJSInProcessRuntime
    {
        private readonly IJSInProcessRuntime inProcessInstance = instance as IJSInProcessRuntime;

        public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, object[] args)
            => instance.InvokeAsync<TValue>(identifier, InterceptArgs(identifier, args));

        public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, CancellationToken cancellationToken, object[] args)
            => instance.InvokeAsync<TValue>(identifier, cancellationToken, InterceptArgs(identifier, args));

        [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed.")]
        public TResult Invoke<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TResult>(string identifier, params object[] args)
            => inProcessInstance.Invoke<TResult>(identifier, args);

        private object[] InterceptArgs(string identifier, object[] args)
        {
            if (identifier == "import")
            {
                return args?.Select(importArg => importArg is string importPath ? ReplaceImportPath(importPath) : importArg).ToArray();
            }
            return args;
        }

        private string ReplaceImportPath(string importPath)
        {
            var splitPaths = importPath.Split('/');
            var replacedImportPath = string.Join('/', splitPaths.Select((path, index) => index < splitPaths.Length - 1 && path.StartsWith('_') ? path[1..] : path));

            if (browserExtensionEnvironment.Mode == BrowserExtensionMode.ContentScript)
            {
                return Path.Combine(browserExtensionEnvironment.BaseUrl, replacedImportPath.TrimStart('/', '.'));
            }

            return replacedImportPath;
        }
    }
}
