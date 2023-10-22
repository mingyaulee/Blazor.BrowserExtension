#nullable enable
// Implementation from https://github.com/dotnet/aspnetcore/blob/main/src/Shared/StaticWebAssets/ManifestStaticWebAssetFileProvider.cs

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazor.BrowserExtension.Build.Tasks.StaticWebAssets
{
    internal sealed class StaticWebAssetManifest
    {
#if NETFRAMEWORK
#pragma warning disable S3400 // Methods should not return constants
        internal static bool IsWindows()
#pragma warning restore S3400 // Methods should not return constants
        {
            // .Net framework runs on Windows
            return true;
        }
#else
        internal static bool IsWindows()
        {
            return OperatingSystem.IsWindows();
        }
#endif

    internal static readonly StringComparer PathComparer =
                 IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

        public string[] ContentRoots { get; set; } = Array.Empty<string>();

        public StaticWebAssetNode Root { get; set; } = null!;

        internal static StaticWebAssetManifest Parse(string manifest)
        {
            return JsonSerializer.Deserialize<StaticWebAssetManifest>(manifest)!;
        }
    }

    internal sealed class StaticWebAssetNode
    {
        [JsonPropertyName("Asset")]
        public StaticWebAssetMatch? Match { get; set; }

        [JsonConverter(typeof(OSBasedCaseConverter))]
        public Dictionary<string, StaticWebAssetNode>? Children { get; set; }

        public StaticWebAssetPattern[]? Patterns { get; set; }

        internal bool HasChildren() => Children != null && Children.Count > 0;

        internal bool HasPatterns() => Patterns != null && Patterns.Length > 0;
    }

    internal sealed class StaticWebAssetMatch
    {
        [JsonPropertyName("ContentRootIndex")]
        public int ContentRoot { get; set; }

        [JsonPropertyName("SubPath")]
        public string Path { get; set; } = null!;
    }

    internal sealed class StaticWebAssetPattern
    {
        [JsonPropertyName("ContentRootIndex")]
        public int ContentRoot { get; set; }

        public int Depth { get; set; }

        public string Pattern { get; set; } = null!;
    }

    internal sealed class OSBasedCaseConverter : JsonConverter<Dictionary<string, StaticWebAssetNode>>
    {
        public override Dictionary<string, StaticWebAssetNode> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var parsed = JsonSerializer.Deserialize<Dictionary<string, StaticWebAssetNode>>(ref reader, options)!;
            var result = new Dictionary<string, StaticWebAssetNode>(StaticWebAssetManifest.PathComparer);
            MergeChildren(parsed, result);
            return result;

            
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<string, StaticWebAssetNode> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }

        private static void MergeChildren(Dictionary<string, StaticWebAssetNode> newChildren, Dictionary<string, StaticWebAssetNode> existing)
        {
            foreach (var keyValuePair in newChildren)
            {
                var key = keyValuePair.Key;
                var value = keyValuePair.Value;
                if (existing.TryGetValue(key, out var existingNode))
                {
                    MergeChildNode(value, existingNode);
                }
                else
                {
                    existing.Add(key, value);
                }
            }
        }

        private static void MergeChildNode(StaticWebAssetNode newNode, StaticWebAssetNode existingNode)
        {
            if (newNode.Patterns != null)
            {
                if (existingNode.Patterns == null)
                {
                    existingNode.Patterns = newNode.Patterns;
                }
                else
                {
                    if (newNode.Patterns.Length > 0)
                    {
                        var newList = new StaticWebAssetPattern[existingNode.Patterns.Length + newNode.Patterns.Length];
                        existingNode.Patterns.CopyTo(newList, 0);
                        newNode.Patterns.CopyTo(newList, existingNode.Patterns.Length);
                        existingNode.Patterns = newList;
                    }
                }
            }

            if (newNode.Children != null)
            {
                if (existingNode.Children == null)
                {
                    existingNode.Children = newNode.Children;
                }
                else
                {
                    if (newNode.Children.Count > 0)
                    {
                        MergeChildren(newNode.Children, existingNode.Children);
                    }
                }
            }
        }
    }
}
