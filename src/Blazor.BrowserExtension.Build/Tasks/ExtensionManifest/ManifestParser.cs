using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazor.BrowserExtension.Build.Tasks.ExtensionManifest
{
    internal static class ManifestParser
    {
        private static JsonSerializerOptions jsonSerializerOptions;

        public static bool TryParseManifestFile(string manifestFile, out Manifest manifest)
        {
            if (jsonSerializerOptions is null)
            {
                jsonSerializerOptions = new JsonSerializerOptions();
                jsonSerializerOptions.Converters.Add(new ManifestJsonConverter());
            }

            try
            {
                var items = JsonSerializer.Deserialize<IDictionary<ManifestItemKey, ManifestItem>>(manifestFile, jsonSerializerOptions);
                manifest = new Manifest()
                {
                    Items = items
                };
                return true;
            }
            catch (JsonException jsonException)
            {
                manifest = new Manifest()
                {
                    ParseErrors = new[]
                    {
                        new ManifestParseError(jsonException.LineNumber, jsonException.BytePositionInLine, jsonException.Message)
                    }
                };
                return false;
            }
        }

        private sealed class ManifestJsonConverter : JsonConverter<IDictionary<ManifestItemKey, ManifestItem>>
        {
            public override IDictionary<ManifestItemKey, ManifestItem> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException();
                }

                var dictionary = new Dictionary<ManifestItemKey, ManifestItem>();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        return dictionary;
                    }

                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException();
                    }

                    var lineNumber = GetLineNumber(ref reader);
                    var columnNumber = GetColumnNumber(ref reader);
                    var propertyName = reader.GetString();
                    reader.Read();
                    var propertyValue = JsonSerializer.Deserialize<JsonElement>(ref reader);

                    var manifestItemKey = MapManifestItemKey(propertyName);
                    if (manifestItemKey is not null)
                    {
                        dictionary.Add(manifestItemKey.Value, new(lineNumber, columnNumber, propertyValue.ToString()));
                    }
                }

                throw new JsonException();
            }

            public override void Write(Utf8JsonWriter writer, IDictionary<ManifestItemKey, ManifestItem> value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }

            private static ManifestItemKey? MapManifestItemKey(string key)
            {
                return key switch
                {
                    "manifest_version" => ManifestItemKey.ManifestVersion,
                    "options_ui" => ManifestItemKey.OptionsUi,
                    "browser_action" => ManifestItemKey.BrowserAction,
                    "background" => ManifestItemKey.Background,
                    "content_security_policy" => ManifestItemKey.ContentSecurityPolicy,
                    "content_scripts" => ManifestItemKey.ContentScripts,
                    "web_accessible_resources" => ManifestItemKey.WebAccessibleResources,
                    "permissions" => ManifestItemKey.Permissions,
                    _ => null,
                };
            }

            delegate long? GetNumberFieldDelegate(Utf8JsonReader reader);
            private static readonly ParameterExpression readerParameter = Expression.Parameter(typeof(Utf8JsonReader), "reader");
            private static readonly Expression getLineNumberField = Expression.Field(readerParameter, "_lineNumber");
            private static readonly Expression returnLineNumberConverted = Expression.ConvertChecked(getLineNumberField, typeof(long?));
            private static readonly Expression<GetNumberFieldDelegate> getLineNumberExpression = Expression.Lambda<GetNumberFieldDelegate>(returnLineNumberConverted, readerParameter);
            private static readonly GetNumberFieldDelegate getLineNumberDelegate = getLineNumberExpression.Compile();
            private static readonly Expression getColumnNumberField = Expression.Field(readerParameter, "_bytePositionInLine");
            private static readonly Expression returnColumnNumberConverted = Expression.ConvertChecked(getColumnNumberField, typeof(long?));
            private static readonly Expression<GetNumberFieldDelegate> getColumnNumberExpression = Expression.Lambda<GetNumberFieldDelegate>(returnColumnNumberConverted, readerParameter);
            private static readonly GetNumberFieldDelegate getColumnNumberDelegate = getColumnNumberExpression.Compile();
            private static long? GetLineNumber(ref Utf8JsonReader reader)
            {
                var lineNumber = getLineNumberDelegate(reader);
                if (lineNumber.HasValue)
                {
                    return lineNumber.Value + 1;
                }
                return null;
            }

            private static long? GetColumnNumber(ref Utf8JsonReader reader)
            {
                return getColumnNumberDelegate(reader);
            }
        }
    }
}
