using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserDesign.UI
{
    class KMLItem: IFileItem
    {
        public KMLItem(string name, string path): base(name, path)
        {
            
        }
        public KMLItem():base("","")
        {
            
        }

    }
}
