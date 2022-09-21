using System.Collections.Generic;

namespace BrowserDesign.UI
{
    public class PortalDirectory: IPortalItem, IContainerItem<IPortalItem>
    {
        List<IPortalItem> layers;
        public override bool Loaded { get; set; } = true; //by default portal directory is always loaded
        public PortalDirectory(string name, string URL)
        {
            base.name = name;
            base.url = URL;
            layers = new List<IPortalItem>();
        }
        public PortalDirectory(string name, string URL, bool requireToken)
        {
            base.name = name;
            base.url = URL;
            layers = new List<IPortalItem>();
            base.requireToken = requireToken;
        }
        public void AddLayer(IPortalItem item)
        {
            if (!layers.Contains(item))
            {
                layers.Add(item);
            }
        }

        public List<IPortalItem> GetLayers()
        {
            return layers;
        }

        public void ClearLayers()
        {
            layers.Clear();
        }
    }
}
