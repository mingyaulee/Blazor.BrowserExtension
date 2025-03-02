using Blazor.BrowserExtension.Build.Tasks.Bootstrap;

namespace Blazor.BrowserExtension.Build.Test.Tasks.Bootstrap
{
    public class ProjectFileBootstrapperTest : BaseFileBootstrapperTest<ProjectFileBootstrapper>
    {
        protected override string OriginalFileContent => """
            <Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">

              <PropertyGroup>
                <TargetFramework>net9.0</TargetFramework>
                <BrowserExtensionBootstrap>true</BrowserExtensionBootstrap>
              </PropertyGroup>

              <ItemGroup>
                <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="9.0.2" />
                <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.DevServer" Version="9.0.2" PrivateAssets="all" />
              </ItemGroup>

            </Project>
            """;

        protected override string BootstrappedFileContent => """
            <Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">
            
              <PropertyGroup>
                <TargetFramework>net9.0</TargetFramework>
              </PropertyGroup>
            
              <ItemGroup>
                <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="9.0.2" />
                <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.DevServer" Version="9.0.2" PrivateAssets="all" />
              </ItemGroup>
            
            </Project>
            """;
    }
}
