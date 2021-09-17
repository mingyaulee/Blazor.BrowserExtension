using System.Xml.Serialization;

namespace Blazor.BrowserExtension.Build.Tasks.StaticWebAssets
{
    public class XmlContentRoot
    {
        [XmlAttribute("BasePath")]
        public string BasePath { get; set; }

        [XmlAttribute("Path")]
        public string Path { get; set; }
    }
}
