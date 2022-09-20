using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserDesign.UI
{
    /// <summary>
    /// Base interface for data containers that are used for data processing and items
    /// </summary>
    public abstract class IBaseItem
    {
        //item status loaded by manager
        public virtual bool Loaded { get; set; } = false;
        //name of the item
        protected string name;
        public string Name { get { return name; } }
    }
}
