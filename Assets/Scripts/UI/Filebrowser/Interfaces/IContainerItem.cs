using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserDesign.UI
{
    interface IContainerItem<T>: IContainerLabelItem
    {
        List<T> GetLayers();
        void AddLayer(T layer);
        void ClearLayers();
    }

    // this interface simply for labeling the generic interface IContainerItem, for easy to identify type
    interface IContainerLabelItem
    {

    }
}
