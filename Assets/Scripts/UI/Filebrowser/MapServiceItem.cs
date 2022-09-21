using System.Collections.Generic;

namespace BrowserDesign.UI
{
    class MapServiceItem: IPortalItem, IContainerItem<IPortalItem>
    {
        private List<IPortalItem> layers;

        //info about the mapservice, uniquely to service item
        public string Description { get; set; }
        public string Snippt { get; set; }
        public MapServiceItem(string name, string url)
        {
            base.name = name;
            base.url = url;
            layers = new List<IPortalItem>();
        }
        public MapServiceItem(string name, string url, bool requireToken)
        {
            base.name = name;
            base.url = url;
            layers = new List<IPortalItem>();
            base.requireToken = requireToken; 
        }
        public void AddLayer(IPortalItem layer)
        {
            if (layers.Find(i => i.URL == layer.URL) != null)
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
            if ((result = layers.Find(i => i.URL == layer.URL)) != null)
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
