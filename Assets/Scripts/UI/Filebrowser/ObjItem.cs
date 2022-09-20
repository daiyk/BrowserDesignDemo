using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserDesign.UI
{
    class ObjItem : IFileItem
    {
        public ObjItem(string name, string path): base(name,path)
        {
   
        }
        public ObjItem():base("","")
        {

        }

    }
}