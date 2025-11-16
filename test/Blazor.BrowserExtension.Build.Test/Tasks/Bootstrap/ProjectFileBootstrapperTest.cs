using Blazor.BrowserExtension.Build.Tasks.Bootstrap;

namespace Blazor.BrowserExtension.Build.Test.Tasks.Bootstrap
{
    [TestClass]
    public class ProjectFileBootstrapperTest : BaseFileBootstrapperTest<ProjectFileBootstrapper>
    {
        protected override string OriginalFileContent => """
            <Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">

              <PropertyGroup>
                <TargetFramework>netX.0</TargetFramework>
                <BrowserExtensionBootstrap>true</BrowserExtensionBootstrap>
              </PropertyGroup>

              <ItemGroup>
                <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="X.Y.Z" />
                <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.DevServer" Version="X.Y.Z" PrivateAssets="all" />
              </ItemGroup>

            </Project>
            """;

        protected override string BootstrappedFileContent => """
            <Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">
            
              <PropertyGroup>
                <TargetFramework>netX.0</TargetFramework>
              </PropertyGroup>
            
              <ItemGroup>
                <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="X.Y.Z" />
                <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.DevServer" Version="X.Y.Z" PrivateAssets="all" />
              </ItemGroup>
            
            </Project>
            """;
    }
}
