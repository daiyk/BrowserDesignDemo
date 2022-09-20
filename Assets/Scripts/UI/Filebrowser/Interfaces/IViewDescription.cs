using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserDesign.UI
{
    public interface IViewDescription
    {
        ViewItem Item { get; set; }
        void InitializeDescription(ViewItem it, ItemLoadBrowser fileBrowserController);
        void LoadItem();
        void UnLoadItem();
    }
}
