using System.Collections.Generic;
using System.Xml.Serialization;

namespace Blazor.BrowserExtension.Build.Tasks.StaticWebAssets
{
    [XmlRoot("StaticWebAssets")]
    public class XmlStaticWebAssetsRoot
    {
        [XmlElement("ContentRoot")]
        public List<XmlContentRoot> ContentRoots { get; set; }
    }
}
