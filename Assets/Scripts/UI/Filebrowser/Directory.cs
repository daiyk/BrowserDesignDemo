using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserDesign.UI
{
    public class Directory: IFileItem,IContainerItem<IFileItem>
    {
        //files it contains
        List<IFileItem> items;
        public override bool Loaded { get; set; } = true; //by default directory is loaded = true
        public Directory(string name, string path, List<IFileItem> childItems): base(name, path)
        {
            items = childItems;
        }
        public Directory(string name, string path): base(name, path)
        {
            items = new List<IFileItem>();
        }
        public Directory():base("","")
        {
            items = new List<IFileItem>();
        }

        //Items should have these properties
        public void AddLayer(IFileItem item)
        {
            if (!items.Contains(item))
            {
                items.Add(item);
            }
        }
        public List<IFileItem> GetLayers()
        {
            return items;
        }
        
        public void ClearLayers()
        {
            items.Clear();
        }
        
    }
}
