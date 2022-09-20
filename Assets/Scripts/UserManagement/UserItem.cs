using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserDesign.users
{
    public class UserItem
    {
        public UserItem(string name, string resource, string type)
        {
            Name = name;
            Resource = resource;
            Type = type;
        }
        public string Name { get; set; }
        public string Resource { get; set; }
        public string Type { get; set; }
    }
}
