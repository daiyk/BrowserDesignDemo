using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserDesign.UI
{
    /// <summary>
    /// file entity that represents the local resource
    /// </summary>
    public abstract class IFileItem:IBaseItem
    {
        protected string path;

        public IFileItem(string name, string path)
        {
            this.path = path;
            this.name = name;
        }
        public string Path 
        { 
            get { return path; } 
        }
        public override bool Equals(object obj)
        {
            var itemCast = obj as IFileItem;
            if(itemCast != null)
            {
                return itemCast.path == path;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.path.GetHashCode();
        }
    }
}
