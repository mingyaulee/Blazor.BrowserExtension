using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Blazor.BrowserExtension
{
    internal class ProxyJsRuntime : IJSInProcessRuntime
    {
        private readonly IJSRuntime instance;
        private readonly IJSInProcessRuntime inProcessInstance;

        public ProxyJsRuntime(IJSRuntime instance)
        {
            this.instance = instance;
            inProcessInstance = instance as IJSInProcessRuntime;
        }

        public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, object[] args)
        {
            return instance.InvokeAsync<TValue>(identifier, InterceptArgs(identifier, args));
        }

        public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, CancellationToken cancellationToken, object[] args)
        {
            return instance.InvokeAsync<TValue>(identifier, cancellationToken, InterceptArgs(identifier, args));
        }

        public TResult Invoke<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TResult>(string identifier, params object[] args)
        {
            return inProcessInstance.Invoke<TResult>(identifier, args);
        }

        static object[] InterceptArgs(string identifier, object[] args)
        {
            if (identifier == "import")
            {
                return args?.Select(importArg => importArg is string importPath ? ReplaceImportPath(importPath) : importArg).ToArray();
            }
            return args;
        }

        static string ReplaceImportPath(string importPath)
        {
            var splitPaths = importPath.Split('/');
            return string.Join('/', splitPaths.Select((path, index) => index < splitPaths.Length - 1 && path.StartsWith('_') ? path[1..] : path));
        }
    }
}
