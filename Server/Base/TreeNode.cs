//using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace OpenWLS.Server.Base
{
     public class TreeNode
    {
        public string Text { get; set; }
        public string ChildrenName { get; set; }


        [JsonIgnore]
        public bool? Selected { get; set; }

        [JsonIgnore]
        public bool ChildrenVisible
        {
            get
            {
                return (Children != null);
            }
        }
        public List<TreeNode> Children { get; set; }
    }
}
