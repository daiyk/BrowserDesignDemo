using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserDesign.UI
{
    class DXFItem : IFileItem
    {
        public DXFItem(string name, string path): base(name, path)
        {
  
        }
        public DXFItem():base("","")
        {

        }

    }
}
