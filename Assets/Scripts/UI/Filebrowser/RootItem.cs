using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace BrowserDesign.UI
{
    /// <summary>
    /// Root item that serve as a management item which user can add/remove portal and service by clicking this item.
    /// </summary>
    class RootItem: IBaseItem, IContainerItem<IBaseItem>
    {
        List<IBaseItem> childs;
        
        public RootItem(string name)
        {
            base.name = name;
            childs = new List<IBaseItem>();
        }
        public void AddLayer(IBaseItem chi)
        {
            if (!childs.Contains(this))
            {
                childs.Add(chi);
            }
        }
        public void RemoveLayer(IBaseItem chi)
        {
            childs.Remove(chi);
        }
        public List<IBaseItem> GetLayers()
        {
            return childs;
        }

        public void ClearLayers()
        {
            childs.Clear();
        }

    }
}
