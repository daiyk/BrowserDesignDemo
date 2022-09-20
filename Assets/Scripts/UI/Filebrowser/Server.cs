using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserDesign.UI
{
    class Server:IPortalItem, IContainerItem<IPortalItem>
    {
        List<IPortalItem> layers;
        public override bool Loaded { get; set; } = true; //by default server is always a loaded item

        public Server(string name, string URL)
        {
            layers = new List<IPortalItem>();
            base.name = name;
            base.url = URL;
        }
        public Server(PortalDirectory pd)
        {
            layers = pd.GetLayers();
            name = pd.Name;
            url = pd.URL;
        }
        public Server(string name, string URL, bool requireToken)
        {
            layers = new List<IPortalItem>();
            base.name = name;
            base.url = URL;
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
