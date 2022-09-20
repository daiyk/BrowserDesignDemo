using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrowserDesign.UI
{
    class JOBItem : IFileItem
    {
        public JOBItem(string name, string path) : base(name, path)
        {

        }
        public JOBItem() : base("", "")
        {

        }
    } 
}
