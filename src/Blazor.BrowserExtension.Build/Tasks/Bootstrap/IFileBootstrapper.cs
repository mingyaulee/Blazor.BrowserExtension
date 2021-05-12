using System.Collections.Generic;

namespace Blazor.BrowserExtension.Build.Tasks.Bootstrap
{
    public interface IFileBootstrapper
    {
        bool Bootstrap(List<string> fileLines);
    }
}
