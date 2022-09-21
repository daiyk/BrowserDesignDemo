using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace BrowserDesign.UI
{
    /// <summary>
    /// ViewItem is used for View Model: UI in TreeView package
    /// </summary>
    public class ViewItem:IBaseItem
    {
        IBaseItem dataItem;

        //compulsory field for treeView
        public ViewItem Parent;
        public List<ViewItem> Children;
        
        //override the base class name property, since we need to modify it
        public new string Name
        {
            get { return base.name; }
            set { base.name = value; }
        }
        public ViewItem(string name)
        {
            base.name = name;
            Children = new List<ViewItem>();
        }
        public ViewItem(string name, IBaseItem dataIt)
        {
            base.name = name;
            dataItem = dataIt;
            Children = new List<ViewItem>();
        }
        public IBaseItem DataItem
        {
            get { return dataItem; }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
