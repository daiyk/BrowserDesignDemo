using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserDesign.UI
{
    class FeatureServiceItem: IPortalItem, IContainerItem<IPortalItem>
    {
        private List<IPortalItem> layers;

        //info about the feature service
        public string Description { get; set; }
        public string Snippt { get; set; }

        //default don't need token
        public FeatureServiceItem(string name, string url, bool token)
        {
            base.name = name;
            base.url = url;
            layers = new List<IPortalItem>();
            requireToken = token;
        }
        public FeatureServiceItem(string name, string url)
        {
            base.name = name;
            base.url = url;
            layers = new List<IPortalItem>();
        }

        public void AddLayer(IPortalItem layer)
        {
            if(layers.Find(i=>i.URL == layer.URL) != null)
            {
                return;
            }
            else
            {
                layers.Add(layer);
            }
        }

        public void RemoveLayer(IPortalItem layer)
        {
            IPortalItem result;
            if((result = layers.Find(i => i.URL == layer.URL)) != null)
            {
                layers.Remove(result);
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
