using System.Collections.Generic;
using System.Xml.Serialization;

namespace Blazor.BrowserExtension.Build.Tasks.StaticWebAssets
{
    [XmlRoot("StaticWebAssets")]
    public class StaticWebAssetsRoot
    {
        [XmlElement("ContentRoot")]
        public List<ContentRoot> ContentRoots { get; set; }
    }
}
