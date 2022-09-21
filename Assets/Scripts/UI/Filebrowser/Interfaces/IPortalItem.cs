using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserDesign.UI
{
    public abstract class IPortalItem:IBaseItem
    {
        protected string url;
        protected bool requireToken;
        protected string thumbNailUrl; //url to this item's thumbnail picture in the portal

        //This property is unique for private portal item, since it includes a image link
        public string ThumbNailUrl
        {
            get { return thumbNailUrl; }
            set { thumbNailUrl = value; }
        }

        public bool RequireToken 
        { 
            get { return requireToken; } 
        }

        //portal item should have a webURL that uniquely defines itself
        public string URL
        {
            get { return url; }
        }
        public override bool Equals(object obj)
        {
            var itemCast = obj as IPortalItem;
            if (itemCast != null)
            {
                return itemCast.url == url;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return url.GetHashCode();
        }
    }
}
