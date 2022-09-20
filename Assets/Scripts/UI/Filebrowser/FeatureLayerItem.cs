using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserDesign.UI
{
    class FeatureLayerItem: IPortalItem
    {
        IPortalItem parent;
        public IPortalItem Parent
        {
            get { return parent; }
            set
            {
                if(value is FeatureServiceItem || value is WebMapItem || value is MapServiceItem)
                {
                    parent = value;
                }
            }
        }
        public String Description { get; set; }
        public FeatureLayerItem(string name, string url)
        {
            base.name = name;
            base.url = url;
        }
        public FeatureLayerItem(string name, string url, bool token)
        {
            base.name = name;
            base.url = url;
            requireToken = token;
        }
        
    }
}
